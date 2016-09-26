//** YouTube Gallery Wall- By Dynamic Drive DHTML code library: http://www.dynamicdrive.com
//** Script Download/ instructions page: http://www.dynamicdrive.com/dynamicindex17/youtube-gallery-wall.htm
//** Created: April 12, 2016
//** Last modified: April 15th, 16 v1.1 to fix iOS devices sometimes not loading videos

var youtubeapidfd = $.Deferred();

// define onYouTubeIframeAPIReady() function
function onYouTubeIframeAPIReady(){
    youtubeapidfd.resolve();
}
	
(function($){

    var KEYCODE_ESC = 27;

    var thumbnailformat = 'http://img.youtube.com/vi/VIDEOID/0.jpg';

	var ytubelightbox = 
		'<div class="videobox">'
		+ '<div class="centeredchild">'
		+ '<div class="videowrapper">'
		+ '<div id="videoplayer"></div>'
		+ '</div>'
		+ '</div>'
		+ '</div>';

//// End Configuration here ////

	var defaults = {};
	var $videobox;
	var youtubeplayer;
	var ismobile = navigator.userAgent.match(/(iPad)|(iPhone)|(iPod)|(android)|(webOS)/i) != null; //boolean check for popular mobile browsers
	var isiOS = navigator.userAgent.match(/(iPad)|(iPhone)|(iPod)/i) != null; //boolean check for iOS devices
	
	function getyoutubeid(link){
		// Assumed Youtube URL formats
		// https://www.youtube.com/watch?v=Pe0jFDPHkzo
		// https://youtu.be/Pe0jFDPHkzo
		// https://www.youtube.com/v/Pe0jFDPHkzo
		// and more
	     
		//See http://stackoverflow.com/a/6904504/4360074
		var youtubeidreg = /(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/ ]{11})/i;
		return youtubeidreg.exec(link)[1]; // return Youtube video ID portion of link
	}

	function createlightbox(){
		$videobox = $(ytubelightbox).appendTo(document.body);

		// Hide lightbox when clicked on
	    $videobox.on('click',
	        function() {
	            hidelightbox();
	        });
		
		// Exclude youtube iframe from above action
	    $videobox.find('.centeredchild')
	        .on('click',
	            function(e) {
	                e.stopPropagation();
	            });
	}

	function showlightbox() {
	    $(document.documentElement).addClass('hidescrollbar');
	    $videobox.css({ display: 'block' });
	}

	function hidelightbox() {
	    $(document.documentElement).removeClass('hidescrollbar');
	    $videobox.css({ display: 'none' });
	    youtubeplayer.stopVideo();
	}
	
	// load Youtube API code asynchronously
	var tag = document.createElement('script');
	
	tag.src = "https://www.youtube.com/iframe_api";
	var firstScriptTag = document.getElementsByTagName('script')[0];
	firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

	youtubeapidfd.then(function(){
	    createlightbox();
		$(document).on('keyup', function(e){
			if (typeof $videobox != 'undefined'){
				if (e.keyCode == KEYCODE_ESC)
				    hidelightbox();
			}
		})
	})

	
	// Creates a new YT.Player() instance
	function createyoutubeplayer(videourl, containerid){
		youtubeplayer = new YT.Player(containerid, {
			videoId: videourl,
			playerVars: {autoplay:1}
		});
	}

	jQuery.fn.youtubegallery = function(options) {
        var s = $.extend({}, defaults, options)
        return this.each(function() { //return jQuery obj
            var $ul = $(this);
            $ul.addClass('youtubewall');
            var $lis = $ul.find('li');
            $lis.each(function(i) {
                var $li = $(this);
                var link = $li.find('a').get(0);
                var caption=$li.find('a').data('name');
                var videoid = getyoutubeid(link.getAttribute('href'));
                var thumbnail = thumbnailformat.replace('VIDEOID', videoid);
                var doclink = link.getAttribute('data-url');
                if (ismobile) {
                    $li.css({ cursor: 'pointer' });
                }
                $li.html(
                    '<div class="thumbwrap">' +
                    '<img src="' +
                    thumbnail +
                    '" />' +
                    '<div class="panel"><div class="panelinner"><i class="play fa fa-play-circle-o"></i> <a class="externallink fa fa-external-link-square" href="' +
                    doclink +
                    '"></a></div></div>' +
                    '</div>'
                    + ((caption != "")? '<div class="caption">' + caption + '</div>' : "")
                );
                if (!doclink) {
                    $li.find('.externallink').css({ display: 'none' });
                }
                $li.find('.panel')
                    .find('.play')
                    .on('click',
                        function() {
                            if (typeof $videobox != 'undefined') {
                                showlightbox();
                                if (typeof youtubeplayer == 'undefined') {
                                    createyoutubeplayer(videoid, 'videoplayer');
                                } else {
                                    if (isiOS) {
                                        youtubeplayer.cueVideoById(videoid);
                                    } else {
                                        youtubeplayer.loadVideoById(videoid);
                                    }
                                }
                            }
                        });
            });
        });
    }

})(jQuery);