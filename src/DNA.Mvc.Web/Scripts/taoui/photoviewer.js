/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    ///<summary>Use to view photos and comments in album</summary>
    $.widget("dna.taoPhotoViewer", {
        options: {
            src: null,
            items: null,
            itemAttr: "href",
            index: 0,
            close: null,
            loadingText: "Loading photo data...",
            closeText:"Close"
        },
        _create: function () {
            var self = this, opts = this.options,
                eventPrefix = this.widgetEventPrefix,
                el = this.element, _photos = [];
            this._unobtrusive();

            //if (!opts.src) {
            if (opts.items && opts.itemAttr) {
                //opts.src = [];
                opts.items.each(function (i, n) {
                    //opts.src.push($(n).attr(opts.itemAttr));
                    _photos.push($(n).attr(opts.itemAttr));
                });
                this.photos = _photos;
            }
            //}

            var viewer = $("<div/>").css("zIndex", $.topMostIndex()).appendTo("body");

            viewer.addClass("d-photo-viewer").disableSelection();
            this.viewer = viewer;

            if (opts.close)
                el.bind(eventPrefix + "close", opts.close);

            /**Header**/
            $("<div/>").addClass("d-photo-viewer-header")
                             .appendTo(viewer)
                             .append($("<a/>").attr("role", "link")
                                                          .text("["+opts.closeText+"]")
                                                          .click(function () {
                                                              $(window).unbind("keyup");
                                                              self.destroy();
                                                              self._triggerEvent("close");
                                                          }));

            var _sidebar = $("<div/>").addClass("d-photo d-sidebar d-tran")
                                                      .appendTo(viewer);
            this._sidebar = _sidebar;
            _sidebar.hide();

            this._container = $("<div/>").addClass("d-photo-container").attr("tabIndex", 0).appendTo(viewer);
            this._setSize();
            window.addEventListener("resize", $.proxy(self._setSize, self), false);

            //if (opts.src) {
            //    if ($.isArray(opts.src)) {
            if (_photos.length) {
                if (opts.src)
                    opts.index = _photos.indexOf(opts.src);
                //opts.index=_photos
                //this._loadPhoto(opts.src[opts.index]);
                this._loadPhoto(_photos[opts.index]);

                this._container.on("mouseup", function (event) {
                    if (!self.isDagging && event.button != 0) {
                        event.preventDefault();
                        var x = event.clientX;
                        if (x > $(this).width() / 2)
                            self.next();
                        else
                            self.prev();
                    }
                });

                $("<div/>").addClass("d-photo-nav-left d-tran")
                                  .appendTo(viewer)
                                  .append($("<span/>").addClass(" d-icon-arrow-left-2"))
                                  .click(function () { self.prev(); });

                $("<div/>").addClass("d-photo-nav-right d-tran")
                                  .appendTo(viewer)
                                  .append($("<span/>").addClass("d-icon-untitled"))
                                  .click(function () { self.next(); });
            }
            else {
                if (opts.src)
                    this._loadPhoto(opts.src);
            }
            //}

            //this._buildCollectorInfo(_sidebar, opts.photo);
            //this._buildComment(_sidebar, opts.photo);
            //this._buildImg(this._container, opts.photo);
            //this._buildImgTools(this._container, opts.photo);
            //this._buildTools(this._container, opts.photo);

            this.proportion = 1;
            this._container.on("mousewheel", function (event, delta, deltaX, deltaY) {
                //console.log(delta);
                if (self.proportion < 0.3 && delta > 0)
                    return;

                if (self.proportion > 4 && delta < 0)
                    return;

                self.proportion -= (delta / 10);
                self._scale(self.proportion);
            })
                                    .on("dblclick", function () {
                                        var img = $(">img", this);
                                        if (img.length) {
                                            self.proportion = 1;
                                            self._scale(1);
                                            img.position({
                                                my: "center center",
                                                at: "center center",
                                                of: self._container
                                            });

                                        }
                                    })
                                    .bind("contextmenu", function () { return false; });

            $(window).bind("keyup", function (event) {

                if (event.keyCode == 27) {
                    $(window).unbind("keyup");
                    self.destroy();
                    self._triggerEvent("close");
                }

                if (_photos.length) {
                    console.log(event.keyCode);
                    //if ($.isArray(opts.src)) {
                    if (event.keyCode == 37) { self.prev(); }
                    if (event.keyCode == 39) { self.next(); }
                }

            });

        },
        next: function () {
            var opts = this.options;
            opts.index++;

            if (opts.index >= this.photos.length)
                opts.index = 0;

            this._loadPhoto(this.photos[opts.index]);
        },
        prev: function () {
            var opts = this.options;

            if (opts.index == 0)
                opts.index = this.photos.length - 1; 
            else
                opts.index--;

            this._loadPhoto(this.photos[opts.index]);
        },
        _loadPhoto: function (src) {
            var self = this;
            this._container.empty();

            var pLoader = $("<div>").appendTo(this._container).text(this.options.loadingText).addClass("d-photo-loader");
            var img = new Image();
            $(img).bind("load", function () {
                pLoader.remove();
                $(this).appendTo(self._container)
                     .position({
                         my: "center center",
                         at: "center center",
                         of: self._container
                     }).animate({ "opacity": "1" }, 500).draggable({ start: function () { self.isDagging = true; }, stop: function () { self.isDagging = false; } });

                if ($(this).width() > self._container.width() || $(this).height() > self._container.height()) {
                    self.proportion = .6;
                    self._scale(self.proportion);
                }

            }).css({
                "max-height": this._container.height() + "px",
                "max-width": this._container.widget + "px"
            });

            img.src = src;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("items")) opts.items = el.datajQuery("items");
            if (el.data("src-attr")) opts.itemAttr = el.data("src-attr");
            if (el.attr("data-index") != undefined) opts.index =parseInt(el.attr("data-index"));
            if (el.data("src"))
                opts.src = el.data("src");

            if (el.data("loading-text"))
                opts.loadingText = el.data("loading-text");
            if (el.data("close-text"))
                opts.closeText = el.data("close-text");
        },
        _setSize: function () {
            //var el = this.element;
            //if (el.isVisible()) {
            $("body").css("overflow", "hidden");
            var h = $(window).height() - 50, w = $(window).width();
            this.viewer.height($(window).height()).width(w);

            var _tools = $(".d-photo-tools", this.viewer);
            //var commentContainer = $(">.d-comments-container", el);
            var _sidebar = this._sidebar;
            var _container = this._container;

            //$(">.d-comments-container", el);

            if (_sidebar.isVisible()) {
                //                    $('>.d-img', el).width(w - _sidebar.outerWidth(true) - 10)
                //                                        .height(h)
                //                                        .taoImage("refresh");
                _container.height(h)
                                 .width(w - _sidebar.outerWidth(true) - 10)
                    .css({
                        "line-height": h + "px"
                    });
                _sidebar.height(h);
                _tools.css({
                    left: ((_container.width() / 2) - (_tools.width() / 2)) + "px"
                });
            } else {

                //                    $('>.d-img', el).width(w - 10)
                //                                        .height(h)
                //                                        .taoImage("refresh");
                _container.height(h).width(w - 10).css({
                    "line-height": h + "px"
                });
                _sidebar.height(h);
            }

            //                wrapper.css({
            //                    "margin-top": ((parentHeight / 2) - (clientHeight / 2)) + "px"
            //                });

            //   }
        },
        _buildImg: function (parent, dataItem) {
            var self = this, ratio = dataItem.width / dataItem.height,
            parentHeight = $(parent).height(),
            parentWidth = $(parent).width()
            clientHeight = dataItem.height,
            clientWidth = dataItem.width,
            wrapper = $("<div/>").addClass("d-photo-wrapper").appendTo(parent);

            var faceImg = $("<img/>").appendTo(wrapper),
            backImg = $("<img/>").appendTo(wrapper).hide();

            if (clientHeight > clientWidth) {
                if (clientHeight > parentHeight) {
                    clientHeight = parentHeight;
                    clientWidth = Math.round(clientHeight * ratio);
                }
            }
            else {
                if (clientWidth > parentWidth) {
                    clientWidth = parentWidth;
                    clientHeight = Math.round(parentWidth / ratio);
                }
            }
            clientHeight = clientHeight - 10;
            clientWidth = clientWidth - 10;

            faceImg.width(clientWidth).height(clientHeight);
            backImg.width(clientWidth).height(clientHeight);

            wrapper.width(clientWidth)
                         .height(clientHeight)
                         .css({ "margin-top": ((parentHeight / 2) - (clientHeight / 2)) + "px" });

            faceImg.attr("src", dataItem.url + "?w=300&h=" + dataItem.height + "&ratio=true&enlage=false");
            backImg.one("load", function () {
                backImg.show();
                faceImg.animate({ opacity: 0 }, function () { faceImg.hide(); });
            })
                         .bind("dblclick", function () {
                             self._scale(1);
                             wrapper.css({
                                 "margin-top": ((parentHeight / 2) - (clientHeight / 2)) + "px",
                                 "top": "0px",
                                 "left": "0px"
                             });
                         })
                         .attr("src", dataItem.url);

            wrapper.addClass("d-tran-fast");
        },
        _scale: function (ratio) {
            var _wrapper = $(">img", this._container);

            _wrapper.css({
                "transform": "scale(" + ratio + ")",
                "-webkit-transform": "scale(" + ratio + ")",
                "-o-transform": "scale(" + ratio + ")",
                "-moz-transform": "scale(" + ratio + ")"
            });

            // if (!_wrapper.hasClass("ui-draggable"))
            //   _wrapper.draggable();
            //if (ratio > 1)
            //    _wrapper.draggable();
            //else {
            //    if (_wrapper.hasClass("ui-draggable"))
            //        _wrapper.draggable("destroy");
            //}
        },
        _rotate: function (deg) {
            var _wrapper = $(">.d-photo-wrapper", this._container);
            this._scale(1);

            _wrapper.css({
                "transform": "rotateZ(" + deg + "deg)",
                "-webkit-transform": "rotateZ(" + deg + "deg)",
                "-o-transform": "rotateZ(" + deg + "deg)",
                "-moz-transform": "rotateZ(" + deg + "deg)"
            });
        },
        _buildHeader: function (parent, txt) {
            return $("<h2/>").addClass("d-header2").appendTo(parent).text(txt);
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        destroy: function () {
            //this.element.removeClass("d-photo-viewer").empty();
            this.viewer.remove();
            $("body").css("overflow", "auto");
            window.removeEventListener("resize", $.proxy(self._setSize, self), false);
            $.Widget.prototype.destroy.call(this);
            //$.dna.taoDataBindable.prototype.destroy.call(this);
        }
    });
})(jQuery);