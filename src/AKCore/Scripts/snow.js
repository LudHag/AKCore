const init = () => {
  const holder = document.querySelector("#firework");

  if (!holder) {
    return;
  }

  const today = new Date();
  const newYear = new Date(2025, 0, 1);
  const msLeft = newYear - today;
  let hrLeft = msLeft / (1000 * 60 * 60);

  if (hrLeft < -24) {
    return;
  }

  if (hrLeft < 0) {
    hrLeft = 0;
  }
  const intensity = Math.pow(40 - hrLeft, 1.89);

  let w = (holder.width = window.innerWidth),
    h = (holder.height = window.innerHeight);
  const gl = holder.getContext("webgl", { preserveDrawingBuffer: true }),
    webgl = {},
    opts = {
      projectileAlpha: 0.8,
      projectileLineWidth: 1.3,
      fireworkAngleSpan: 0.5,
      baseFireworkVel: 3,
      addedFireworkVel: 3,
      gravity: 0.03,
      lowVelBoundary: -0.2,
      xFriction: 0.995,
      baseShardVel: 1,
      addedShardVel: 0.2,
      fireworks: intensity,
      baseShardsParFirework: 10,
      addedShardsParFirework: 10,
      shardFireworkVelMultiplier: 0.3,
      initHueMultiplier: 1 / 360,
      runHueAdder: 0.1 / 360,
    };

  webgl.vertexShaderSource = `
uniform int u_mode;
uniform vec2 u_res;
attribute vec4 a_data;
varying vec4 v_color;

vec3 h2rgb( float h ){
	return clamp( abs( mod( h * 6. + vec3( 0, 4, 2 ), 6. ) - 3. ) -1., 0., 1. );
}
void clear(){
	gl_Position = vec4( a_data.xy, 0, 1 );
	v_color = vec4( 0, 0, 0, a_data.w );
}
void draw(){
	gl_Position = vec4( vec2( 1, -1 ) * ( ( a_data.xy / u_res ) * 2. - 1. ), 0, 1 );
	v_color = vec4( h2rgb( a_data.z ), a_data.w );
}
void main(){
	if( u_mode == 0 )
		draw();
	else
		clear();
}
`;
  webgl.fragmentShaderSource = `
precision mediump float;
varying vec4 v_color;

void main(){
	gl_FragColor = v_color;
}
`;

  webgl.vertexShader = gl.createShader(gl.VERTEX_SHADER);
  gl.shaderSource(webgl.vertexShader, webgl.vertexShaderSource);
  gl.compileShader(webgl.vertexShader);

  webgl.fragmentShader = gl.createShader(gl.FRAGMENT_SHADER);
  gl.shaderSource(webgl.fragmentShader, webgl.fragmentShaderSource);
  gl.compileShader(webgl.fragmentShader);

  webgl.shaderProgram = gl.createProgram();
  gl.attachShader(webgl.shaderProgram, webgl.vertexShader);
  gl.attachShader(webgl.shaderProgram, webgl.fragmentShader);

  gl.linkProgram(webgl.shaderProgram);
  gl.useProgram(webgl.shaderProgram);

  webgl.dataAttribLoc = gl.getAttribLocation(webgl.shaderProgram, "a_data");
  webgl.dataBuffer = gl.createBuffer();

  gl.enableVertexAttribArray(webgl.dataAttribLoc);
  gl.bindBuffer(gl.ARRAY_BUFFER, webgl.dataBuffer);
  gl.vertexAttribPointer(webgl.dataAttribLoc, 4, gl.FLOAT, false, 0, 0);

  webgl.resUniformLoc = gl.getUniformLocation(webgl.shaderProgram, "u_res");
  webgl.modeUniformLoc = gl.getUniformLocation(webgl.shaderProgram, "u_mode");

  gl.viewport(0, 0, w, h);
  gl.uniform2f(webgl.resUniformLoc, w, h);

  gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
  gl.enable(gl.BLEND);

  gl.lineWidth(opts.projectileLineWidth);

  webgl.data = [];

  webgl.clear = function () {
    gl.uniform1i(webgl.modeUniformLoc, 1);
    const a = 0.1;
    webgl.data = [
      -1,
      -1,
      0,
      a,
      1,
      -1,
      0,
      a,
      -1,
      1,
      0,
      a,
      -1,
      1,
      0,
      a,
      1,
      -1,
      0,
      a,
      1,
      1,
      0,
      a,
    ];
    webgl.draw(gl.TRIANGLES);
    gl.uniform1i(webgl.modeUniformLoc, 0);
    webgl.data.length = 0;
  };
  webgl.draw = function (glType) {
    gl.bufferData(
      gl.ARRAY_BUFFER,
      new Float32Array(webgl.data),
      gl.STATIC_DRAW,
    );
    gl.drawArrays(glType, 0, webgl.data.length / 4);
  };

  let tick = 0;
  const fireworks = [],
    sins = [],
    coss = [],
    maxShardsParFirework =
      opts.baseShardsParFirework + opts.addedShardsParFirework,
    // eslint-disable-next-line @typescript-eslint/no-loss-of-precision
    tau = 6.283185307179586476925286766559;

  for (let i = 0; i < maxShardsParFirework; ++i) {
    sins[i] = Math.sin((tau * i) / maxShardsParFirework);
    coss[i] = Math.cos((tau * i) / maxShardsParFirework);
  }

  function Firework() {
    this.reset();
    this.shards = [];
    for (let i = 0; i < maxShardsParFirework; ++i)
      this.shards.push(new Shard(this));
  }
  Firework.prototype.reset = function () {
    const angle = -Math.PI / 2 + (Math.random() - 0.5) * opts.fireworkAngleSpan,
      vel = opts.baseFireworkVel + opts.addedFireworkVel * Math.random();

    this.mode = 0;
    this.vx = vel * Math.cos(angle);
    this.vy = vel * Math.sin(angle);

    this.x = Math.random() * w;
    this.y = h;

    this.hue = tick * opts.initHueMultiplier;
  };
  Firework.prototype.step = function () {
    if (this.mode === 0) {
      const ph = this.hue,
        px = this.x,
        py = this.y;

      this.hue += opts.runHueAdder;

      this.x += this.vx *= opts.xFriction;
      this.y += this.vy += opts.gravity;

      webgl.data.push(
        px,
        py,
        ph,
        opts.projectileAlpha * 0.2,
        this.x,
        this.y,
        this.hue,
        opts.projectileAlpha * 0.2,
      );

      if (this.vy >= opts.lowVelBoundary) {
        this.mode = 1;

        this.shardAmount =
          (opts.baseShardsParFirework +
            opts.addedShardsParFirework * Math.random()) |
          0;

        const baseAngle = Math.random() * tau;
        let x = Math.cos(baseAngle);
        let y = Math.sin(baseAngle);
        const sin = sins[this.shardAmount];
        const cos = coss[this.shardAmount];

        for (let i = 0; i < this.shardAmount; ++i) {
          const vel = opts.baseShardVel + opts.addedShardVel * Math.random();
          this.shards[i].reset(x * vel, y * vel);
          const X = x;
          x = x * cos - y * sin;
          y = y * cos + X * sin;
        }
      }
    } else if (this.mode === 1) {
      this.ph = this.hue;
      this.hue += opts.runHueAdder;

      let allDead = true;
      for (let i = 0; i < this.shardAmount; ++i) {
        const shard = this.shards[i];
        if (!shard.dead) {
          shard.step();
          allDead = false;
        }
      }

      if (allDead) this.reset();
    }
  };
  function Shard(parent) {
    this.parent = parent;
  }
  Shard.prototype.reset = function (vx, vy) {
    this.x = this.parent.x;
    this.y = this.parent.y;
    this.vx = this.parent.vx * opts.shardFireworkVelMultiplier + vx;
    this.vy = this.parent.vy * opts.shardFireworkVelMultiplier + vy;
    this.starty = this.y;
    this.dead = false;
    this.tick = 1;
  };
  Shard.prototype.step = function () {
    this.tick += 0.05;

    const px = this.x,
      py = this.y;

    this.x += this.vx *= opts.xFriction;
    this.y += this.vy += opts.gravity;

    webgl.data.push(
      px,
      py,
      this.parent.ph,
      opts.projectileAlpha / this.tick,
      this.x,
      this.y,
      this.parent.hue,
      opts.projectileAlpha / this.tick,
    );

    if (this.y > h) this.dead = true;
  };

  function anim() {
    window.requestAnimationFrame(anim);

    webgl.clear();

    ++tick;

    if (fireworks.length < opts.fireworks) fireworks.push(new Firework());

    fireworks.map(function (firework) {
      firework.step();
    });

    webgl.draw(gl.LINES);
  }
  anim();

  window.addEventListener("resize", function () {
    w = holder.width = window.innerWidth;
    h = holder.height = window.innerHeight;

    gl.viewport(0, 0, w, h);
    gl.uniform2f(webgl.resUniformLoc, w, h);
  });
  window.addEventListener("click", function (e) {
    const firework = new Firework();
    firework.x = e.clientX;
    firework.y = e.clientY;
    firework.vx = 0;
    firework.vy = 0;
    fireworks.push(firework);
  });
};
init();
