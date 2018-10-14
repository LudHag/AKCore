<template>
    <div id="spooky-container" class="hidden-xs" v-bind:class="{ fly }" @click="fly = !fly">
        <div id="spooky">
            <div id="body">
                <div id="eyes"></div>
                <div id="mouth"></div>
                <div id="feet">
                    <div></div>
                    <div></div>
                    <div></div>
                </div>
            </div>
        </div>
        <div id="shadow"></div>
    </div>
</template>
<script>
    export default {
        data: function () {
            return {
                fly: false
            }
        }
    }
</script>
<style lang="scss" scoped>
    #spooky-container {
        width: 400px;
        height: 400px;
        position: absolute;
        left: 0vw;
        top: -25px;
        z-index: 22;
        padding: 0;
        cursor: pointer;
        box-sizing: border-box;
        transform: scale(.4);
        transform-origin: top left;
        &.fly
        {
            animation: flighty 5s ease-in-out infinite;
        }
    }

*:after,
*:before {
  box-sizing: inherit;
}

#spooky {
  margin: 10% auto;
  width: 80%;
  height: 80%;
  animation: floaty 2s infinite;
}

#spooky #body {
  position: relative;
  margin: 50px auto 0;
  width: 180px;
  height: 220px;
  background: #f2fbf1;
  border-top-left-radius: 90px;
  border-top-right-radius: 90px;
}

#spooky #body:before,
#spooky #body:after {
  content: "";
  position: absolute;
  top: 130px;
  display: inline-block;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: #f2fbf1;
  animation: floaty 0.2s infinite;
}

#spooky #body:before {
  left: -18px;
}

#spooky #body:after {
  right: -18px;
}

#spooky #body #eyes {
  display: flex;
  justify-content: space-between;
  margin: 0 auto;
  padding: 90px 0 0;
  width: 90px;
  height: 20px;
}

#spooky #body #eyes:before,
#spooky #body #eyes:after {
  content: " ";
  display: block;
  width: 30px;
  height: 30px;
  background: #252c49;
  border-radius: 50%;
}

#spooky #body #mouth {
  background: #252c49;
  margin: 35px auto 0;
  width: 60px;
  height: 30px;
  border-bottom-left-radius: 30px;
  border-bottom-right-radius: 30px;
}

#spooky #body #mouth:before {
  content: " ";
  display: block;
  background: #fff;
  margin-left: 20px;
  width: 10px;
  height: 10px;
}

#spooky #body #feet {
  position: absolute;
  display: flex;
  bottom: -18px;
  width: 180px;
  height: 36px;
}

#spooky #body #feet > *,
#spooky #body #feet::before,
#spooky #body #feet::after {
  content: " ";
  width: 36px;
  height: 36px;
  background: #f2fbf1;
  border-radius: 50%;
}

#shadow {
  margin: -90px auto 0;
  background: #252c49;
  width: 180px;
  height: 40px;
  border-radius: 50%;
  animation: zoomy 2s infinite;
}

@keyframes floaty {
  0%,
  100% {
    transform: translateY(0);
  }

  50% {
    transform: translateY(-20px);
  }
}

@keyframes zoomy {
  0%,
  100% {
    transform: scale(1);
  }

  50% {
    transform: scale(0.8);
  }
}

    @keyframes flighty {
        0%, 100% {
            left: 0vw;
            top: -25px;
        }

        25% {
            left: 25vw;
            top: 60vh;
        }
        50% {
            left: 60vw;
            top: 40vh;
        }
        75% {
            left: 40vw;
            top: 15vh;
        }
    }
</style>
