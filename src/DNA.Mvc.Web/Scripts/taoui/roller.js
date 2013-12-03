/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoRoller", {
        options: {
            direction: "left",
            active: null,
            autoplay: false,
            speed: 300,
            interval: 3000,
            nextOnClick: true,
            navigator: false,
            offset: 0,
            buttons:false,
            activeIndex: 0
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();

            if (opts.active)
                el.bind(eventPrefix + "active", opts.active);

            //el.addClass("d-roller");
            el.addClass("d-roller").addClass("d-roller-" + opts.direction);

            if (el.height() == 0) el.height(350);

            if (el.children().length == 1) {
                el.children().css({ height: "inherit", width: "100%" });
                return el;
            }

            if (el.children().length >= 3) {
                el.children().each(function (i, nel) {
                    $(nel).position({
                        at: "right middle",
                        my: "left middle",
                        of: el,
                        collision: "none"
                    });
                });
                //var sp = opts.speed;
                //opts.speed = 0;
                this._left(el.children(":eq(0)"));
                this._center(el.children(":eq(1)"));
                this._right(el.children(":eq(2)"));
                //opts.speed = sp;
            }

            if (el.children().length == 2) {
                this._left(el.children(":eq(1)"));
                this._center(el.children(":eq(0)"));
            }

            if (opts.nextOnClick) {
                el.children().click(function (event) {
                    event.preventDefault();
                    if (el.children().length == 2) { self.next(); } else {
                        if (el.children().index(this) == 1)
                            self.next();
                        else
                            self.go($(this).data("index"));
                    }
                });
            }

            el.children().on("mouseenter", function () { self.pused = true; })
                                .on("mouseleave", function () { self.pused = false; })
                                .css({
                                    position: "absolute",
                                    height: "inherit",
                                    width: opts.offset > 0 ? (((el.width() - (2 * opts.offset) - 10) / el.width()) * 100) + "%" : "100%"
                                }).each(function (i, child) {
                                    $(child).attr("data-index", i);
                                });

            window.addEventListener("resize", function () {
                if (el && el.length)
                    el.children().css({ height: "inherit", width: opts.offset > 0 ? (((el.width() - (2 * opts.offset) - 10) / el.width()) * 100) + "%" : "100%" });
            }, false);

            this._createNavigator();

            if (opts.autoplay)
                self.play();

            self.go(opts.activeIndex);
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;
            if (el.data("dir")) opts.direction = el.data("dir");
            if (el.data("offset") != undefined) opts.offset = el.dataInt("offset");
            if (el.data("index") != undefined) opts.activeIndex = el.dataInt("index");
            if (el.data("speed") != undefined) opts.speed = el.dataInt("speed");
            if (el.data("autoplay") != undefined) opts.autoplay = el.dataBool("autoplay");
            if (el.data("nav") != undefined) opts.navigator = el.data("nav");
            if (el.data("interval") != undefined) opts.interval = el.dataInt("interval");
            if (el.data("active")) opts.scroll = new Function("event", "ui", el.data("active"));
            if (el.data("next-onclick") != undefined) opts.nextOnClick = el.dataBool("next-onclick");

            if (el.data("next")) {
                var nav_next = el.datajQuery("next");
                nav_next.click(function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    self.next();
                });
            }

            if (el.data("prev")) {
                var nav_prev = el.datajQuery("prev");
                nav_prev.click(function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    self.previous();
                });
            }
        },
        puse: function () {
            this.pused = true;
            return this.element;
        },
        resume: function () { this.pused = false; return this.element; },
        play: function (interval) {
            if (this._playing) return this.element;
            var self = this;

            if (!this.element.isVisible()) return;

            if (interval)
                this.options.interval = interval;

            this._playing = window.setInterval(function () {
                if (!self.pused)
                    self.next();
            }, this.options.interval ? this.options.interval : 3000);

            this._playing = null;
            return this.element;
        },
        stop: function () {
            if (this._playing) {
                window.clearInterval(this._playing);
                this._playing = null;
            }
            return this.element;
        },
        _animate: function (to) {
            $(this).stop(true, false).animate(to);
        },
        _left: function (element) {
            var self = this, opts = this.options;
            element.position({
                my: opts.direction == "left" ? "right center" : "center bottom",
                at: opts.direction == "left" ? "left center" : "center top",
                offset: opts.direction == "left" ? (this.options.offset + "px 0") : ("0 " + this.options.offset + "px"),
                of: this.element,
                collision: "none",
                using: function (to) {
                    if (opts.direction == "left")
                        to.top = 0;
                    $(this).stop(true, false).animate(to, opts.speed);
                }
            })
        },
        _right: function (element) {
            var self = this, opts = this.options;
            return element.position({
                at: opts.direction == "left" ? "right center" : "center bottom",
                my: opts.direction == "left" ? "left center" : "center top",
                offset: opts.direction == "left" ? ("-" + this.options.offset + "px 0") : ("0 -" + this.options.offset + "px"),
                of: this.element,
                collision: "none",
                using: function (to) {
                    $(this).stop(true, false).animate(to, opts.speed);
                }
            });
        },
        _center: function (element) {
            var self = this, opts = this.options;
            return element.position({
                my: "center center",
                at: "center center",
                of: this.element,
                using: function (to) {
                    $(this).stop(true, false).animate(to, opts.speed);
                }
            });
        },
        _createNavigator: function () {
            var el = this.element, eventPrefix = this.widgetEventPrefix, self = this, opts = this.options;
            if (el.children().length > 1 && opts.navigator != "none" && opts.navigator != false) {
                var nav = $("<ul/>").addClass("d-page-navigator");
                el.children().each(function (i, slide) {
                    //var s = $(slide);
                    var slide = $("<li/>").attr("data-index", i).appendTo(nav).click(function () {
                        if (!$(this).isActive())
                            self.go(i);
                    });

                    if (i == opts.activeIndex)
                        slide.isActive(true);
                });

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
        go: function (index) {
            var el = this.element,
                opts = this.options,
                targetItem = el.children("[data-index=" + index + "]");

            if (targetItem.length) {
                var len = el.children("[data-index]").length,
                    currentIndex = len == 2 ? el.children(":eq(0)").data("index") : el.children(":eq(1)").data("index");

                // console.log(index);

                if (index == currentIndex)
                    return;

                var currentItem = el.children("[data-index=" + currentIndex + "]");

                if (len == 2) {
                    this._left(currentItem);
                    this._center(targetItem);
                    currentItem.appendTo(el);
                } else {

                    //Step 2 : we need to know is roll to next or prev

                    var targetPos = el.children().index(targetItem), isNext = targetPos > 1 ? true : false;

                    if (isNext) { //roll to next (to left or top)
                        this._left(currentItem);
                    } else { //roll to prev
                        this._right(currentItem);
                    }

                    this._center(targetItem);

                    if (isNext) { //left
                        //var lastIndex = currentItem.next().data["index"];
                        var ci = el.children().index(currentItem);
                        el.children(":lt(" + ci + ")").position({
                            at: opts.direction == "left" ? "right center" : "center bottom",
                            my: opts.direction == "left" ? "left center" : "center top",
                            of: el,
                            collision: "none"
                        })
                            .appendTo(el);

                        this._right(targetItem.next());
                        //this._right(el.children("[data-index=" + lastIndex + "]"));

                    } else { //right
                        //var firstIndex = currentItem.prev().data["index"];
                        var ci = el.children().index(currentItem);
                        el.children(":gt(" + ci + ")").position({
                            my: opts.direction == "left" ? "right center" : "center bottom",
                            at: opts.direction == "left" ? "left center" : "center top",
                            of: el,
                            collision: "none"
                        }).prependTo(el);

                        this._left(targetItem.prev());
                        //this._left(el.children("[data-index=" + firstIndex + "]"));
                    }
                }

                this.options.activeIndex = index;
                //Move the third item
                this._triggerEvent("active", { item: targetItem, index: index });
            }

            return this.el;
        },
        next: function () {
            var el = this.element, opts = this.options, len = el.children().length;
            if (len == 2)
                this.go(opts.activeIndex == 0 ? 1 : 0);
            else {
                if ((opts.activeIndex + 1) >= len)
                    this.go(0);
                else
                    this.go(opts.activeIndex + 1);
            }
            return el;
        },
        previous: function () {
            var el = this.element, opts = this.options, len = el.children().length;
            if (len == 2) {
                this.go(opts.activeIndex == 0 ? 1 : 0);
            } else {
                if ((opts.activeIndex - 1) <= 0)
                    this.go(len - 1);
                else
                    this.go(opts.activeIndex - 1);
            }
            return this.element;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        disable: function () {
            this.widget().addClass("d-state-disable");
            return this;
        },
        enable: function () {
            this.widget().removeClass("d-state-disable");
            return this;
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);