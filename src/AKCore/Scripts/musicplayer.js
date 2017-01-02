$(function() {
    $('.music-player').each(function() {
        var self = $(this);
        var albums = widgetAlbums[self.data('id')];
        new MusicPlayer(self, albums);
    });
});

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
        self.resetPlayer();
        self.renderElement();
    });
    this.container.on('click', '.playlist-element', function (e) {
        e.preventDefault();
        var player = self.container.find('.player');
        var link = $(this).attr('href');
        player.attr('src', link);
        player.trigger('play');
    });

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
    albumKeys.forEach(function(key) {
        var album = self.albums[key];
        albumsContainer.append('<div class="album-element"><a class="album-link" data-id="' + key +'" href="#"><img class="album-img" src="' + album.image + '" /><p class="album-name">' + album.name + '</p></a></div>');
    });
    this.element.append(albumsContainer);
};
MusicPlayer.prototype.createPlayer = function () {
    this.player = $('<audio controls class="player" src=""><p>Your browser does not support the <code>audio</code> element.</p></audio>');
    var playerModule = $('<div class="player-module"></div>');
    playerModule.append(this.player);
    this.playListModule = $('<div class="playlist"></div>');
    playerModule.append(this.playListModule);
    var playerContainer = $('<div class="player-container"></div>');
    this.albumDisplay = $('<div class="album-display"><img class="album-display-image" src="" /></div>');
    playerContainer.append(this.albumDisplay);
    playerContainer.append(playerModule);
    this.element.append(playerContainer);
};
MusicPlayer.prototype.selectAlbum = function (id) {
    this.currentAlbumId = id;
    this.selectedAlbum = this.albums[id];
    var displayImage = this.albumDisplay.find('.album-display-image');
    displayImage.attr('src', this.selectedAlbum.image);
    this.playList = this.selectedAlbum.tracks;
    this.buildPlayList();
};


MusicPlayer.prototype.buildPlayList = function () {
    var self = this;
    this.playListModule.empty();
    var keys = Object.keys(this.playList);
    keys.forEach(function (el) {
        var track = self.playList[el];
        self.playListModule.append(self.createListElement(el, track.name));
    });
};
MusicPlayer.prototype.createListElement = function (number, name) {
    return $('<a href="/albums/' + this.currentAlbumId + '/' + name + '" class="playlist-element"><span class="number">' + number + '. </span><span class="name">' + name + '</span></a>');
};

MusicPlayer.prototype.renderElement = function () {
    this.container.append(this.element);
};
