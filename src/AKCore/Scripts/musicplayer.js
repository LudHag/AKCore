$(function() {
    $('.music-player').each(function() {
        var self = $(this);
        var albums = widgetAlbums[self.data('id')];
        new MusicPlayer(self, albums);
    });
});
function fmtMSS(s) {
    if (isNaN(s)) {
        return '0:00';
    }
    s = Math.floor(s);
    return (s - (s %= 60)) / 60 + (9 < s ? ':' : ':0') + s;
}
function MusicPlayer(container, albums) {
    this.container = container;
    this.albums = albums;
    this.playList = {};
    this.element = $('<div></div>');
    this.createPlayer();
    this.createAlbums();
    this.selectAlbum(Object.keys(albums)[0]);
    this.renderElement();
    this.initBinds();
};

MusicPlayer.prototype.initBinds = function () {
    var self = this;
    this.container.on('click', '.album-link', function(e) {
        e.preventDefault();
        var id = $(this).data('id');
        self.selectAlbum(id);
    });
    function downloadURI(uri, name) {
        var link = document.createElement("a");
        link.download = name;
        link.href = uri;
        link.click();
    }
    this.container.on('click', '.playlist-element', function (e) {
        e.preventDefault();
        if (!$(e.target).hasClass('glyphicon-download')) {
            self.playTrack($(this));
        } else {
            downloadURI($(this).attr('href'), $(this).find('.name').text());
        }
    });
   
    this.container.on('click', '.play-all', function (e) {
        e.preventDefault();
        var els = self.container.find('.playlist-element');
        if(els.length>0){
            els.addClass('queued');
            self.playTrack($(self.container.find('.playlist-element')[0]));

            var player = self.container.find('.player');
            player.off('ended');
            player.on('ended', function (e) {
                e.stopPropagation();
                self.playNext(self.container.find('.playlist-element.active'));
            });
        }
    });
    this.loadPlayer();
};

MusicPlayer.prototype.loadPlayer = function() {
    var player = this.container.find('.player');
    var self = this;
    player.on("timeupdate", function (event) {
        self.updateProgress(event.target.currentTime, this.duration);
    });
    var playpause = this.container.find('.pauseplay');

    playpause.on('click', function(e) {
        e.preventDefault();
        if (playpause.hasClass('glyphicon-play')) {
            playpause.removeClass('glyphicon-play');
            playpause.addClass('glyphicon-pause');
            player.trigger('play');
        } else {
            playpause.addClass('glyphicon-play');
            playpause.removeClass('glyphicon-pause');
            player.trigger('pause');
        }
    });

    player.on('ended', function () {
        playpause.removeClass('glyphicon-pause');
        playpause.addClass('glyphicon-play');
        self.updateProgress(0);
    });

    var progressContainer = this.container.find('.progress-container');

    progressContainer.on('click', function (event) {
        event.preventDefault();
        var decimalProgress = event.offsetX / progressContainer.width();
        var time = decimalProgress * player[0].duration;
        self.updateProgress(time, player[0].duration);
        player[0].currentTime = time;
    });
};

MusicPlayer.prototype.updateProgress = function (time, duration) {
    var progressTime = this.container.find('.progresstime');
    var durationBar = this.container.find('.music-progress-bar');
    var progress = (time / duration) * 100;
    durationBar.css({ 'width': progress + '%' });
    progressTime.text(fmtMSS(time) + '/' + fmtMSS(duration));
};

MusicPlayer.prototype.playNext = function(linkElement) {
    linkElement.removeClass('queued');
    linkElement.removeClass('active');
    var next = linkElement.next();
    if (next.length > 0) {
        if (!next.hasClass('queued')) {
            this.playNext(next);
        } else {
            this.playTrack(next);
        }
    }
}

MusicPlayer.prototype.playTrack = function (linkElement) {
    var player = this.container.find('.player');
    var playingnow = this.container.find('.playingnow');
    var link = linkElement.attr('href');
    playingnow.text('Spelar nu: ' + linkElement.find('span').text());
    player.attr('src', link);
    player.trigger('load');
    player.trigger('play');
    var controls = this.container.find('.controls');
    controls.removeClass('hide');
    var playpause = this.container.find('.pauseplay');
    playpause.removeClass('glyphicon-play');
    playpause.addClass('glyphicon-pause');
    this.container.find('.playlist-element.active').removeClass('active');
    linkElement.addClass('active');
};

MusicPlayer.prototype.resetPlayer = function () {
    var player = this.container.find('.player');
    player.attr('src', '');
    player.trigger('load');
};
MusicPlayer.prototype.createAlbums = function() {
    var self = this;
    var albumsContainer = $('<div class="album-list"></div>');
    var albumKeys = Object.keys(this.albums);
    var nameKeyMap = {};
    albumKeys.forEach(function(key) {
        nameKeyMap[self.albums[key].name] = key;
    });
    Object.keys(nameKeyMap).sort().forEach(function(name) {
        var key = nameKeyMap[name];
        var album = self.albums[key];
        albumsContainer.append('<div class="album-element"><a class="album-link" data-id="' + key + '" href="#"><img class="album-img" src="' + album.image + '" /><p class="album-name">' + album.name + '</p></a></div>');
    });

    this.element.append(albumsContainer);
};
MusicPlayer.prototype.createPlayer = function () {
    var playerModule = $('<div class="player-module"></div>');
    playerModule.append($('<div class="playingnow"></div>'));
    playerModule.append($(
        '<div class="controls hide"><a href="#" class="pauseplay glyphicon glyphicon-play"></a><div class="progress-container"><div class="music-progress"><span class="music-progress-bar" style="width: 0%;"></span></div></div><div class="progresstime"></div></div>'));
    playerModule.append($('<audio class="player" src=""><p>Your browser does not support the <code>audio</code> element.</p></audio>'));
    this.playListModule = $('<div class="playlist"></div>');
    playerModule.append(this.playListModule);
    var playerContainer = $('<div class="player-container"></div>');
    this.albumDisplay = $('<div class="album-display"><img class="album-display-image" src="" /><a href="#" class="play-all">Spela alla spår <span class="glyphicon glyphicon-play"></span></a></div>');
    this.albumTitle = $('<h2 class="album-title"></h2>');
    playerContainer.append(this.albumDisplay);
    playerContainer.append(playerModule);
    this.element.append(this.albumTitle);
    this.element.append(playerContainer);
};
MusicPlayer.prototype.selectAlbum = function (id) {
    this.currentAlbumId = id;
    this.selectedAlbum = this.albums[id];
    var displayImage = this.albumDisplay.find('.album-display-image');
    displayImage.attr('src', this.selectedAlbum.image);
    this.albumTitle.html(this.selectedAlbum.name);
    this.playList = this.selectedAlbum.tracks;
    this.buildPlayList();
};


MusicPlayer.prototype.buildPlayList = function () {
    var self = this;
    this.playListModule.empty();
    var keys = Object.keys(this.playList);
    keys.forEach(function (el) {
        var track = self.playList[el];
        self.playListModule.append(self.createListElement(el, track.filename, track.name));
    });
};
MusicPlayer.prototype.createListElement = function (number, filename, name) {

    return $('<a href="/albums/' + this.currentAlbumId + '/' + filename + '" class="playlist-element"><span class="name">' + name + '</span><span class="glyphicon glyphicon-download"></span></a>');
};

MusicPlayer.prototype.renderElement = function () {
    this.container.append(this.element);
};
