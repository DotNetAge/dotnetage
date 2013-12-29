/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoMenu", $.dna.taoHierarchical, {
        options: {
            type: "horizontal",
            moreTitle: "Toggle navigation",
            moreIcon: "d-icon-reorder",
            alt: true,
            itemClick: null,
            sameWidth: true,
            separators: true,
            childrenIcon: true
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();
            var _class = el.attr("class") ? el.attr("class") : "";
            if (_class) el.removeClass(_class);

            el.addClass("d-reset d-ui-widget d-items d-menu")

            if (!opts.alt && opts.type == "horizontal")
                el.addClass("responsive");

            if (opts.datasource)
                this._setDataSource(opts.datasource);

            if (opts.type)
                el.addClass(opts.type);

            if (_class)
                el.addClass(_class);

            $("li:not('.d-separator')", el).addClass("d-item");
            $("ul", el).addClass("d-ui-widget d-items-wrapper");

            if (opts.type == "toolbar")
                $(">.d-separator", el).height(el.height());

            var _items = $(".d-item:not('.d-state-disable')", el);

            _items.on("mouseenter", function () {
                $(this).addClass("d-state-hover");
                var _children = $(this).children("ul")

                if (_children.length) {
                    var isTop = $(this).parent().hasClass("d-menu") && (opts.type == "horizontal" || opts.type == "toolbar"),
                     winWidth = $(window).width();

                    if (winWidth <= 400) {
                        _children.css("z-index", $.topMostIndex() + 1)
                                       .position({
                                           of: el.parent(),
                                           at: "left bottom",
                                           my: "left top"
                                       });
                    }
                    else {
                        _children.css("z-index", $.topMostIndex() + 1)
                                       .position({
                                           of: $(this),
                                           at: isTop ? "left bottom" : "right top",
                                           my: "left top",
                                           using: function (using) {
                                               $(this).stop(true, false).animate(using, 5);
                                           }
                                       });
                    }
                }
            })
                       .on("mouseleave", function () {
                           $(this).removeClass("d-state-hover");
                       });

            if (!opts.datasource)
                _items.each(function (i, menuEle) {
                    self._initItem(menuEle);
                });

            $(".d-item[data-checked]:not('.d-state-disable')", el).isActive(true);

            if (opts.itemClick)
                el.bind(eventPrefix + "itemClick", opts.itemClick);

            this.altMode = false;
            $(window).on("resize", function () {
                self.refresh();
            });

            self.refresh();
            return el;
        },
        refresh: function () {
            var self = this, opts = this.options, el = this.element,
                _more = el.children(".d-item-more");

            if (opts.type == "horizontal") {
                //Reset all items
                if (_more.length > 0) {
                    _more.children(".d-items-wrapper").children().appendTo(el);
                    _more.remove();
                }

                $(">.d-item", el).css("width", "auto");
                $(">.d-separator", el).remove();

                var winWidth = $(window).width();

                if (opts.alt) { //show altertive toogle menu
                    if (winWidth > 600) {
                        if (this.altMode) {
                            this.altMode = false;
                            //el.children().not(".d-item-more").show();
                            //el.children(".d-item-more").hide();
                            this._setSeparators(opts.separators);
                            this._setSameWidth(opts.sameWidth);
                        }
                    }

                    if (winWidth <= 600) {
                        if (!this.altModel) {
                            var _container = $("<ul/>").addClass("d-ui-widget d-items-wrapper").append(el.children()),
                            _more = $("<li/>").addClass("d-item d-item-more")
                                                 .append($("<a/>").attr("href", "javascript:void(0);").css({ "min-width": "0px", "padding-left": "10px" })
                                                                               //.text(this.options.moreTitle)
                                                                               .append($("<span/>").addClass("d-icon-reorder"))
                                                                )
                                                 .append(_container)
                                                 .click(function () {
                                                     if (this.menuPanel) {
                                                         this.menuPanel.taoPanel("open");
                                                     } else {
                                                         var _menuPanel = $("<div/>").appendTo("body");
                                                         _menuPanel.attr("data-role", "panel").attr("data-icon", "d-icon-reorder")
                                                                               .attr("data-display", "push")
                                                             .attr("data-position", "fixed").css('position','fixed')
                                                                               .attr("title", opts.moreTitle);
                                                         var _mobileMenu = $("<ul/>").append(_container.children(":not(.d-separator)").clone());
                                                         _menuPanel.append(_mobileMenu);

                                                         _mobileMenu.taoListview();
                                                         _mobileMenu.mobilelist();
                                                         _menuPanel.taoPanel({
                                                             opened: true,
                                                             open: function () {
                                                                 _menuPanel.data("overflow", $("body").css("overflow"));
                                                                 $("body").css("overflow", "hidden");
                                                             },
                                                             close: function () {
                                                                 $("body").css("overflow", _menuPanel.data("overflow"));
                                                             }
                                                         });
                                                         this.menuPanel = _menuPanel;
                                                     }

                                                     //$(this).toggleClass("d-state-hover");
                                                     //_container.css("z-index", $.topMostIndex() + 1)
                                                     //    .position({
                                                     //        of: el.parent(),
                                                     //        at: "left bottom",
                                                     //        my: "left top"
                                                     //    });
                                                 })
                                                 .appendTo(el);

                            if (opts.separators) {
                                this._setSeparators(false); //remove separators
                                this._setSeparators(true);
                            }
                            //_container.css("min-width", "300px").width(winWidth < 400 ? winWidth : el.width()).children(".d-item").removeAttr("style");
                            _container.hide();
                            this.altMode = true;
                        }
                    }

                    el.css("overflow", "auto").css("overflow", "visible");
                } else {
                    this._setSeparators(opts.separators);
                    el.children(".d-float-none").removeClass("d-float-none");

                    if (winWidth > 600)
                        this._setSameWidth(opts.sameWidth);

                    if (opts.sameWidth) {
                        if (winWidth > 400 && winWidth <= 600)
                            el.children(".d-item").css("width", "190px");

                        if (winWidth <= 400)
                            el.children(".d-item")
                                .addClass("d-float-none")
                                .css({
                                    "width": "100%"
                                });
                    }

                    if (winWidth > 400 && winWidth < 600 && el.hasClass("responsive")) {
                        el.children(".d-item").removeAttr("style");
                    }

                    if (winWidth <= 400)
                        $(".d-items-wrapper", el).css("min-width", "auto");
                    //this._setSameWidth(opts.sameWidth);
                }

            } else {
                if (opts.type != "toolbar") {
                    this._setSeparators(opts.separators);
                    this._setSameWidth(opts.sameWidth);
                }
            }

        },
        _isOverflow: function () {
            var self = this, opts = this.options, el = this.element,
                overflowIndex = -1;

            if (opts.type == "horizontal" || opts.type == "toolbar") {
                var totalWidth = 0;

                var w = el.width();

                el.children().each(function (i, mi) {
                    var itemWidth = $(mi).outerWidth(true) + ($(mi).outerWidth(true) - $(mi).outerWidth(false));
                    totalWidth += itemWidth;
                    //console.log($(mi).css("width"));
                    //console.log(itemWidth);
                    if (totalWidth > w && overflowIndex == -1)
                        overflowIndex = i;
                });

                //console.log(totalWidth);
                //console.log(el.outerWidth(true));

                var endWidth = 0;
                if (overflowIndex > -1) {
                    var dispWidth = 0;
                    el.children(":lt(" + overflowIndex + ")").each(function (i, dn) {
                        dispWidth += $(dn).outerWidth(true) + ($(dn).outerWidth(true) - $(dn).outerWidth(false));
                    });

                    endWidth = el.width() - dispWidth;
                    //console.clear();
                    //console.log(endWidth);
                    if (endWidth < 50) {
                        overflowIndex--;
                        if (el.children().eq(overflowIndex).hasClass("d-separator"))
                            overflowIndex--;
                    }
                }

                // var lastItemWidth = (el.width() - (el.children(".d-separator").length * 5)) - (itemWidth * (el.children(".d-item:not(.d-item-more)").length - 1));
                return {
                    width: el.width(),
                    itemsWidth: totalWidth,
                    //endWidth: endWidth,
                    index: overflowIndex,
                    item: overflowIndex > -1 ? el.children().eq(overflowIndex) : null
                };
            } else {
                var totalHeight = 0;
                el.children().each(function (i, mi) {
                    totalHeight += $(mi).outerHeight(true);
                    if (totalHeight > el.height() && overflowIndex == -1)
                        overflowIndex = i;
                });

                return {
                    height: el.height(),
                    itemsHeight: totalHeight,
                    index: overflowIndex,
                    item: overflowIndex > -1 ? el.children().eq(overflowIndex) : null
                };
            }

        },
        _setSameWidth: function (val) {
            var self = this, opts = this.options, el = this.element;
            if (opts.type == "vertical")
                return;

            if (val) {
                var w = el.width(),
                    count = el.children(".d-item:not(.d-item-more)").length,
                    sw = 0,
                    maxWidth = 0;

                el.children(".d-separator").each(function (i, sp) {
                    if ($(sp).isVisible()) {
                        sw = sw + $(sp).outerWidth(true) + ($(sp).outerWidth(true) - $(sp).outerWidth(false));
                    }
                });

                //console.log(sw);
                //If we using the avgWidth we must remove the totalMarginWidth
                var totalMargin = 0;
                el.children(".d-item").each(function (i, mn) {
                    totalMargin += $(mn).outerWidth(true) - $(mn).outerWidth(false);
                });

                //In deference browsers the avgWidth will not be right! In FF it always get a wrong value!
                var avgWidth = (w - sw - totalMargin) / count;
                if ($(".d-item-more", el).length) {
                    var moreWidth = $(".d-item-more", el).outerWidth(true) + ($(".d-item-more", el).outerWidth(true) - $(".d-item-more", el).outerWidth(false));
                    avgWidth = (w - moreWidth - totalMargin - sw) / count;
                    //console.log(moreWidth);
                }

                avgWidth = avgWidth - 5;

                //Get item max width
                el.children(".d-item").each(function (i, mn) {
                    var curWidth = $(mn).outerWidth(true) + ($(mn).outerWidth(true) - $(mn).outerWidth(false));
                    if (curWidth > maxWidth)
                        maxWidth = curWidth;
                });

                var itemWidth = avgWidth > maxWidth ? (avgWidth) : maxWidth,
                    ctxWidth = (itemWidth / 16) + "em";

                el.children(".d-item:not(.d-item-more)").css("width", ctxWidth);
                //console.log($(".d-items-wrapper", el));
                $(".d-items-wrapper", el).css("min-width", ctxWidth);

            } else {
                el.children(".d-item").css("width", "auto");
                $(".d-items-wrapper", el).css("min-width", "auto");
            }

        },
        _setSeparators: function (val) {
            var opts = this.options, el = this.element;
            if (val) {
                $(".d-item:not(:last-of-type)", this.element).each(function (i, n) {
                    if ($(n).next(".d-separator").length == 0) {
                        var sp = $("<li class='d-separator'/>");
                        $(n).after(sp);
                        if ($(n).parent().hasClass("d-menu") && opts.type == "horizontal") {
                            //console.log($(el).outerHeight(true));
                            //if (this.altMode)
                            //    sp.height(1);
                            //else
                            //    sp.height($(n).height());
                        }
                    }
                })
            } else {
                $(".d-separator", this.element).remove();
            }
        },
        _setOption: function (key, value) {
            if (key == "separators" && this.options.separators != value) {
                this._setSeparators(value);
                this.options.separators = value;
            }

            //if (key == "more") {
            //    if ($(".d-item-more", el).length) {
            //        $(".d-item-more", el).text(value);
            //    }
            //    this.options.more = value;
            //}

            //if (key == "sameWidth" && this.options.sameWidth != value) {
            //    this._setSameWidth(value);
            //    this.options.sameWidth = value;
            //}

            return $.dna.taoDataBindingList.prototype._setOption.call(this, key, value);
        },
        _initItem: function (element) {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix, item = $(element);
            if (item.children("ul").length) {
                item.addClass("hasChildren");
                if (opts.childrenIcon) {
                    var _c = $("<span/>").addClass((item.parent().hasClass("horizontal") || item.parent().hasClass("toolbar")) ? "d-icon-caret-down" : "d-icon-caret-right ")
                                                       .appendTo(item.children("a"))
                                                       .addClass("d-children-icon");
                }
            }
            if (!item.hasClass("d-item"))
                item.addClass("d-item");
            item.bind("click", function (e) {
                if (opts.type == "toolbar" && !$(this).data("role") && $(">a[href]", item).length == 0) {
                    e.stopPropagation();
                    e.preventDefault();
                }

                if ($(this).data("role") == "checkbox") {
                    if ($(this).attr("data-checked") != undefined) {
                        $(this).removeAttr("data-checked")
                        $(this).isActive(false);
                    }
                    else {
                        $(this).attr("data-checked", true);
                        $(this).isActive(true);
                    }
                } else {
                    if ($(this).data("role") == "radio") {
                        if ($(this).attr("data-group") != undefined) {
                            var group = $(this).attr("data-group");
                            $(".d-item[data-role=radio][data-group='" + group + "']:not('.d-state-disable')", el).removeAttr("data-checked").isActive(false);
                        }
                        else {
                            $(".d-item[data-role=radio]:not('.d-state-disable')", el).removeAttr("data-checked").isActive(false);
                        }

                        $(this).attr("data-checked", true).isActive(true);
                    }
                }

                el.trigger(eventPrefix + "itemClick", { item: $(this) });
            });

        },
        _onItemCreated: function (item, dat) {
            if ($.isPlainObject(dat) && dat.text == "-") {
                item.empty();
                item.addClass("d-separator");
                return this;
            } else
                return $.dna.taoHierarchical.prototype._onItemCreated.call(this, item, dat);
        },
        //_onDataChanged: function (results) {
        //    if (this.emptyElement) {
        //        this.emptyElement.remove();
        //    }
        //    var _items = this._createDataElements(results.data);
        //    this._getItemsContainer().taoUI();

        //    if (this._getItemsContainer().children().length == 0)
        //        this._setEmpty(true);
        //},
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            $.dna.taoDataBindingList.prototype._unobtrusive.call(this, el);
            if (el.data("separators") != undefined) opts.separators = el.dataBool("separators");
            if (el.data("item-click")) opts.itemClick = new Function("event", "ui", el.data("item-click"));
            if (el.data("same-width") != undefined) opts.sameWidth = el.dataBool("same-width");
            if (el.data("more-icon") != undefined) opts.moreIcon = el.data("more-icon");
            if (el.data("more-text") != undefined) opts.moreTitle = el.data("more-text");
            if (el.data("alt") != undefined) opts.alt = el.dataBool("alt");

            if (el.attr("data-role") == "menubar")
                opts.type = "horizontal"
            else {
                if (el.attr("data-role") == "menu")
                    opts.type = "vertical";
                if (el.attr("data-role") == "toolbar")
                    opts.type = "toolbar";
            }
            return this;
        }
    });
})(jQuery);