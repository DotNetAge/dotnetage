/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoContentSlider", {
        options: {
            buttons: false,
            nav: false,
            index: 0,
            speed: 300,
            autoplay: false,
            interval: 3000,
            actived: null,
            itemWidth: 0,
            draggable: true
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            if (opts.actived)
                el.bind(eventPrefix + "active", opts.actived);

            var isUl = el[0].tagName.toLowerCase() == "ul" ? true : false;

            el.addClass("d-content-slider");
            var viewport = $(isUl ? "<li/>":"<div/>").appendTo(el).addClass("d-content-slider-viewport"),
                itemsContainer = $(isUl ? "<ul/>" :"<div/>").appendTo(viewport).addClass("d-content-slider-items");

            el.children(":not(.d-content-slider-viewport)").appendTo(itemsContainer).addClass("d-content-slider-item");

            if (opts.buttons) {
                var prevEl = $("<a/>").attr("href", "javascript:void(0);")
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                        e.preventDefault();
                                                        self.next();
                                                    })
                                                    .addClass("d-nav-prev")
                                                    .append($("<span>").addClass("d-icon-arrow-left-2"))
                                                    .prependTo(el),

                nextEl = $("<a/>").attr("href", "javascript:void(0);")
                                             .click(function (e) {
                                                 e.stopPropagation();
                                                 e.preventDefault();
                                                 self.prev();
                                             })
                                             .append($("<span>").addClass("d-icon-untitled"))
                                             .addClass("d-nav-next")
                                             .appendTo(el);


                this.prevNav = prevEl.addClass("d-tran-fast").css({ "z-index": $.topMostIndex() });
                this.nextNav = nextEl.addClass("d-tran-fast").css({ "z-index": $.topMostIndex() });
            }

            this.resize();

            if (opts.draggable)
                itemsContainer.draggable({
                    axis: "x",
                    start: function (evnet, ui) {
                        //$(this).data("startAt",Date())
                        $(this).data("left", ui.position.left);
                    },
                    stop: function (event, ui) {

                        if (ui.position.left > 0)
                            return $(this).stop().animate({ left: 0 });

                        var lastSlide = itemsContainer.children(":last");
                        if (ui.position.left < (-lastSlide.position().left)) {
                            if (opts.index == lastSlide.index())
                                return $(this).stop().animate({ left: (-lastSlide.position().left) });
                            else
                                return self.go(lastSlide.index());
                        }

                        var curLeft = ui.position.left,
                            curIndex = 0,
                            dir = $(this).data("left") > curLeft ? "l" : "r";

                        $(this).data("left", null);

                        var children = $(".d-content-slider-item", el);

                        for (var i = 0; i < children.length; i++) {
                            var si = children.eq(i),
                                starts = -si.position().left,
                               ends = -(si.position().left + si.width());

                            if (dir == "l") {
                                if (curLeft < starts)
                                    curIndex = i + 1;
                            } else {
                                if (curLeft > ends) {
                                    curIndex = i;
                                    break;
                                }
                            }
                        }

                        return self.go(curIndex);
                    }
                });

            if (opts.nav)
                this._createNavigator();

            itemsContainer.on("mouseenter", function () { self.puse(); })
                .on("mouseleave", function () { self.resume(); });

            window.addEventListener("resize", function () {
                self.puse();
                self.resize();
                self.go(opts.index);
                self.resume();
            }, false);

            if (opts.autoplay)
                this.play();
        },
        play: function () {
            var self = this;
            if (this.token == undefined) {
                this.pused = false;
                this.token = window.setInterval(function () {
                    if (!self.pused) {
                        if (self.options.index < ($(".d-content-slider-item", self.element).length - 1)) {
                            self.next();
                        } else {
                            self.go(0);
                        }
                    }
                }, this.options.interval);
            }
        },
        puse: function () { if (this.token != undefined) { this.pused = true; } },
        resume: function () { if (this.token != undefined) { this.pused = false; } },
        stop: function () {
            if (this.token != undefined) {
                window.clearInterval(this.token);
                this.token = undefined;
                this.pused = null;
            }
        },
        resize: function () {
            /// <summary>Resize the items</summary>
            var el = this.element,
                opts = this.options,
                viewport = $(".d-content-slider-viewport", el),
                itemsContainer = $(".d-content-slider-items", el)
            h = el.height(),
            w = el.width(),
            itemCount = itemsContainer.children().length,
            totalWidth = itemCount * w;

            viewport.height(h).width(w);

            var mHeight = $(".d-content-slider-item", el).eq(0).marginHeight();
            //console.log(mHeight);

            $(".d-content-slider-item", el).height(h - mHeight).width(w);

            if (opts.itemWidth)
                $(".d-content-slider-item", el).width(opts.itemWidth);

            itemsContainer.width(totalWidth);
        },
        next: function () {
            return this.go(this.options.index + 1);
        },
        prev: function () {
            return this.go(this.options.index - 1);
        },
        go: function (index) {
            var _slide = $(".d-content-slider-item", this.element).eq(index);
            $(".d-content-slider-item", this.element).removeClass("d-state-active");
            if (_slide.length) {
                var pos = _slide.position();
                pos.index = index;
                _slide.addClass("d-state-active");
                if (this.options.index != index) {
                    this.options.index = index;
                    this._triggerEvent("active", { index: index });
                }
                return $(".d-content-slider-items", this.element).stop(true, false)
                                                                                              .animate({ left: -_slide.position().left }, this.options.speed);
            } else
                return this.element;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        _createNavigator: function () {
            var el = this.element, eventPrefix = this.widgetEventPrefix, self = this,
                opts = this.options,
                itemCount = $(".d-content-slider-item", el).length;

            if (itemCount > 1 && opts.navigator != "none" && opts.navigator != false) {
                var nav = $("<ul/>").addClass("d-page-navigator");
                for (var i = 0; i < itemCount; i++) {
                    //var s = $(slide);
                    var marker = $("<li/>").attr("data-index", i).appendTo(nav).click(function () {
                        if (!$(this).isActive())
                            self.go($(this).dataInt("index"));
                    });

                    if (i == opts.index)
                        marker.isActive(true);
                }

                nav.css({ "z-index": $.topMostIndex() });

                if (opts.navigator == "top")
                    el.before(nav);
                else
                    el.after(nav);

                el.bind(eventPrefix + "active", function (event, ui) {
                    nav.children().removeClass("d-state-active");
                    nav.children("[data-index=" + ui.index + "]").isActive(true);
                })

                this.nav = nav;
            }
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("buttons") != undefined) opts.buttons = el.dataBool("buttons");
            if (el.data("index") != undefined) opts.activeIndex = el.dataInt("index");
            if (el.data("speed") != undefined) opts.speed = el.dataInt("speed");
            if (el.data("autoplay") != undefined) opts.autoplay = el.dataBool("autoplay");
            if (el.data("draggable") != undefined) opts.draggable = el.dataBool("draggable");
            if (el.data("nav") != undefined) opts.nav = el.data("nav");
            if (el.data("interval") != undefined) opts.interval = el.dataInt("interval");
            if (el.data("active")) opts.scroll = new Function("event", "ui", el.data("active"));
            if (el.data("item-width") != undefined) opts.itemWidth = el.dataInt("item-width");

        }
    });
})(jQuery);