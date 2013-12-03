/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoTabs", {
        options: {
            active: 0,
            prefix: "d-tabs-",
            fill: false
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            el.addClass("d-reset d-ui-widget  d-tabs");
            var tabsHolder = $(">ul", el).addClass("d-tabs-navs");
            $(">ul>li", el).addClass("d-tabs-nav d-ui-widget-header");

            var navHeight = $(".d-tabs-nav", el).eq(0).height();

            var tabsWidth = 0;
            tabsHolder.children().each(function (i, tab) {
                tabsWidth += $(tab).outerWidth(true);
                var link = $("a", tab), tabName = link.attr("href");
                link.disableSelection();
                if (tabName && !tabName.startsWith("#")) {
                    var name = opts.prefix, counter = 0;
                    while ($("#" + name + counter).length > 0)
                        counter++;
                    link.attr("href", "#" + name + counter);

                    var beforeEl = $(">div", el).eq(i - 1),
                        divHolder = $("<div id=\"" + name +counter +"\" />").addClass("d-tabs-panel").attr("data-url", tabName);
                    if (beforeEl.length)
                        beforeEl.after(divHolder);
                    else
                        divHolder.appendTo(el);
                }
            });

            var itemHeight = tabsHolder.children().eq(0).height();

            tabsHolder.width(tabsWidth)
                               .height(itemHeight);

            if (tabsWidth > el.width())
                tabsHolder.draggable({
                    axis: "x",
                    stop: function (event, ui) {
                        if (ui.position.left >= 0)
                            return $(this).stop(true, false).animate({ left: 0 });
                        var limitX = -(tabsHolder.width() - el.width());
                        if (ui.position.left < limitX)
                            $(this).stop(true, false).animate({ left: limitX });
                    }
                });
            else
                tabsHolder.width(el.width());

            var placeHolder = $("<div/>").height(el.height() - itemHeight);
            $(">div", el).addClass("d-tabs-panel").appendTo(placeHolder);
            placeHolder.appendTo(el);

            placeHolder.taoContentSlider({
                actived: function (event, ui) {
                    $(".d-tabs-nav.d-state-active", tabsHolder).isActive(false);
                    $(".d-tabs-nav", tabsHolder).eq(ui.index).isActive(true);
                    var _tn = $(".d-tabs-nav.d-state-active", tabsHolder).children("a").attr("href");

                    if ($(_tn).attr("data-url")) {
                        $(_tn).load($(_tn).attr("data-url"), function () {
                            $(_tn).taoUI();
                            $(_tn).removeAttr("data-url");
                        });
                    }

                }
            });

            $(".d-tabs-nav", tabsHolder).on("click", function (e) {
                e.stopPropagation();
                e.preventDefault();
                if ($(this).isDisable())
                    return el;

                placeHolder.taoContentSlider("go", $(this).index());

                //var thisTab = $(this),
                //    _setActive = function () {
                //        thisTab.siblings(".d-state-active")
                //                     .removeClass("d-ui-widget-content")
                //                     .removeClass("d-state-active");
                //        $(">.d-tabs-panel:visible", el).hide().isActive(false);
                //        thisTab.addClass("d-ui-widget-content").isActive(true);
                //    },

                // tabName = thisTab.children("a").attr("href");
                //if (tabName && !tabName.startsWith("#")) {
                //    thisTab.append($("<span/>").addClass("d-icon-loading"));
                //    var name = opts.prefix, counter = 0;
                //    while ($("#" + name + counter).length > 0)
                //        counter++;
                //    var nTab = $("<div id='" + name + counter + "'/>").appendTo(el).load(tabName, function () {
                //        thisTab.children("a").attr("href", "#" + name + counter);
                //        thisTab.children(".d-icon-loading").remove();
                //        _setActive();
                //        nTab.taoUI();
                //        nTab.addClass("d-tabs-panel d-ui-widget-content").show().isActive(true);
                //    });
                //} else {
                //    _setActive();
                //    $(tabName).show().isActive(true);
                //}
            })
                                 .on("mouseenter", function () {
                                     if (!$(this).isActive() && !$(this).isDisable())
                                         $(this).isHover(true);
                                 })
                                 .on("mouseleave", function () { $(this).isHover(false); }).disableSelection();


            if (opts.active) {
                $(".d-tabs-nav", tabsHolder).eq(opts.active).isActive(true);
                placeHolder.taoContentSlider("go", opts.active);
                //$(tab.children("a").attr("href")).show();//.isActive(true);
            }
            else {
                $(">ul>li:first", el).addClass("d-ui-widget-content").isActive(true);
            }

        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("active") != undefined) opts.active = el.data("active");
            if (el.data("fill") != undefined) opts.fill = el.dataBool("fill");
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        disable: function () {
            this.widget().isDisable(true);
            return this;
        },
        enable: function () {
            this.widget().isDisable(false);
            return this;
        },
        _setOption: function (key, value) {
            return $.Widget.prototype._setOption.call(this, key, value);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);