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
    this.element = $('<div></div>');
    this.createAlbums();
    this.renderElement();
};

MusicPlayer.prototype.createAlbums = function() {
    var self = this;
    var albumsContainer = $('<div class="album-list"></div>');
    var albumKeys = Object.keys(this.albums);
    albumKeys.forEach(function(key) {
        var album = self.albums[key];
        albumsContainer.append('<div class="album-element"><a href="#"><img class="album-img" src="' + album.image + '" /><p class="album-name">' + album.name + '</p></a></div>');
    });
    this.element.append(albumsContainer);
};

MusicPlayer.prototype.renderElement = function () {
    this.container.append(this.element);
};