$(function() {
    $(".music-player").each(function() {
        const self = $(this);
        const albums = widgetAlbums[self.data("id")];
        new MusicPlayer(self, albums);
    });
});

function fmtMSS(s) {
    if (isNaN(s)) {
        return "0:00";
    }
    s = Math.floor(s);
    return (s - (s %= 60)) / 60 + (9 < s ? ":" : ":0") + s;
}

function MusicPlayer(container, albums) {
    this.container = container;
    this.albums = albums;
    this.playList = {};
    this.element = $("<div></div>");
    this.createPlayer();
    this.createAlbums();
    this.selectAlbum(Object.keys(albums)[0]);
    this.renderElement();
    this.initBinds();
};

MusicPlayer.prototype.initBinds = function() {
    const self = this;
    this.container.on("click",
        ".album-link",
        function(e) {
            e.preventDefault();
            const id = $(this).data('id');
            self.selectAlbum(id);
        });

    function downloadURI(uri, name) {
        const link = document.createElement("a");
        link.download = name;
        link.href = uri;
        link.click();
    }

    this.container.on("click",
        ".playlist-element",
        function(e) {
            e.preventDefault();
            if (!$(e.target).hasClass("glyphicon-download")) {
                self.playTrack($(this));
            } else {
                downloadURI($(this).attr("href"), $(this).find(".name").text());
            }
        });

    this.container.on('click',
        '.play-all',
        function(e) {
            e.preventDefault();
            const els = self.container.find(".playlist-element");
            if (els.length > 0) {
                els.addClass("queued");
                self.playTrack($(self.container.find(".playlist-element")[0]));

                const player = self.container.find(".player");
                player.off("ended");
                player.on("ended",
                    function(el) {
                        el.stopPropagation();
                        self.playNext(self.container.find(".playlist-element.active"));
                    });
            }
        });
    this.loadPlayer();
};

MusicPlayer.prototype.loadPlayer = function() {
    const self = this;
    const player = this.container.find(".player");
    player.on("timeupdate",
        function(event) {
            self.updateProgress(event.target.currentTime, this.duration);
        });
    const playpause = this.container.find('.pauseplay');

    playpause.on("click",
        function(e) {
            e.preventDefault();
            if (playpause.hasClass("glyphicon-play")) {
                playpause.removeClass("glyphicon-play");
                playpause.addClass("glyphicon-pause");
                player.trigger("play");
            } else {
                playpause.addClass("glyphicon-play");
                playpause.removeClass("glyphicon-pause");
                player.trigger("pause");
            }
        });

    player.on("ended",
        function() {
            playpause.removeClass("glyphicon-pause");
            playpause.addClass("glyphicon-play");
            self.updateProgress(0);
        });

    const progressContainer = this.container.find('.progress-container');

    progressContainer.on("click",
        function(event) {
            event.preventDefault();
            const decimalProgress = event.offsetX / progressContainer.width();
            const time = decimalProgress * player[0].duration;
            self.updateProgress(time, player[0].duration);
            player[0].currentTime = time;
        });
};

MusicPlayer.prototype.updateProgress = function(time, duration) {
    const progressTime = this.container.find(".progresstime");
    const durationBar = this.container.find(".music-progress-bar");
    const progress = (time / duration) * 100;
    durationBar.css({ 'width': progress + "%" });
    progressTime.text(fmtMSS(time) + "/" + fmtMSS(duration));
};

MusicPlayer.prototype.playNext = function(linkElement) {
    linkElement.removeClass("queued");
    linkElement.removeClass("active");
    const next = linkElement.next();
    if (next.length > 0) {
        if (!next.hasClass("queued")) {
            this.playNext(next);
        } else {
            this.playTrack(next);
        }
    }
}

MusicPlayer.prototype.playTrack = function(linkElement) {
    const player = this.container.find(".player");
    const playingnow = this.container.find(".playingnow");
    const link = linkElement.attr("href");
    playingnow.text("Spelar nu: " + linkElement.find("span").text());
    player.attr("src", link);
    player.trigger("load");
    player.trigger("play");
    const controls = this.container.find(".controls");
    controls.removeClass("hide");
    const playpause = this.container.find(".pauseplay");
    playpause.removeClass("glyphicon-play");
    playpause.addClass("glyphicon-pause");
    this.container.find(".playlist-element.active").removeClass("active");
    linkElement.addClass("active");
};

MusicPlayer.prototype.resetPlayer = function() {
    const player = this.container.find(".player");
    player.attr("src", "");
    player.trigger("load");
};
MusicPlayer.prototype.createAlbums = function() {
    const self = this;
    const albumsContainer = $('<div class="album-list"></div>');
    const albumKeys = Object.keys(this.albums);
    const nameKeyMap = {};
    albumKeys.forEach(function(key) {
        nameKeyMap[self.albums[key].name] = key;
    });
    Object.keys(nameKeyMap).sort().forEach(function(name) {
        const key = nameKeyMap[name];
        const album = self.albums[key];
        albumsContainer.append(`
                <div class="album-element">
                    <a class="album-link" data-id="${key}" href="#">
                        <img class="album-img" src="${album.image}" />
                        <p class="album-name">${album.name}</p>
                    </a>
                </div>`);
    });

    this.element.append(albumsContainer);
};
MusicPlayer.prototype.createPlayer = function() {
    const playerModule = $('<div class="player-module"></div>');
    playerModule.append($('<div class="playingnow"></div>'));
    playerModule.append($(
        '<div class="controls hide"><a href="#" class="pauseplay glyphicon glyphicon-play"></a><div class="progress-container"><div class="music-progress"><span class="music-progress-bar" style="width: 0%;"></span></div></div><div class="progresstime"></div></div>'));
    playerModule.append($(
        '<audio class="player" src=""><p>Your browser does not support the <code>audio</code> element.</p></audio>'));
    this.playListModule = $('<div class="playlist"></div>');
    playerModule.append(this.playListModule);
    const playerContainer = $('<div class="player-container"></div>');
    this.albumDisplay =
        $('<div class="album-display"><img class="album-display-image" src="" /><a href="#" class="play-all">Spela alla spår <span class="glyphicon glyphicon-play"></span></a></div>');
    this.albumTitle = $('<h2 class="album-title"></h2>');
    playerContainer.append(this.albumDisplay);
    playerContainer.append(playerModule);
    this.element.append(this.albumTitle);
    this.element.append(playerContainer);
};
MusicPlayer.prototype.selectAlbum = function(id) {
    this.currentAlbumId = id;
    this.selectedAlbum = this.albums[id];
    const displayImage = this.albumDisplay.find(".album-display-image");
    displayImage.attr("src", this.selectedAlbum.image);
    this.albumTitle.html(this.selectedAlbum.name);
    this.playList = this.selectedAlbum.tracks;
    this.buildPlayList();
};


MusicPlayer.prototype.buildPlayList = function() {
    const self = this;
    this.playListModule.empty();
    const keys = Object.keys(this.playList);
    keys.forEach(function(el) {
        const track = self.playList[el];
        self.playListModule.append(self.createListElement(el, track.filename, track.name));
    });
};
MusicPlayer.prototype.createListElement = function(number, filename, name) {
    return $(`<a href="/albums/${this.currentAlbumId}/${filename}" class="playlist-element">
                <span class="name">${name}</span>
                <span class="glyphicon glyphicon-download"></span>
            </a>`);
};

MusicPlayer.prototype.renderElement = function() {
    this.container.append(this.element);
};