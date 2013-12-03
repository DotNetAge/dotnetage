(function ($) {
    $.widget("dna.taoAccordion", {
        options: {
            active: 0,
            collapsed: false,
            duration: 200
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            el.addClass("d-reset d-ui-widget d-accordion");

            if (!opts.collapsed) {
                $(">div", el).each(function (i, div) {
                    if (opts.active != i)
                        $(div).hide();
                    else
                        $(div).addClass("d-state-active");
                });
            } else
                $(">div", el).hide();

            $(">div", el).addClass("d-ui-widget-content");
            $(">h3", el).wrapInner("<a href=\"javascript:void(0);\"/>");

            //jQuery 1.9.1 $(">h3", el).on("click", function () { });

            $(">h3", el).on("click", function () {
                if (!$(this).isDisable())
                    self._setActive($(this));
            })
                .addClass("d-ui-widget-header")
                .disableSelection()
                .on("mouseenter", function () {
                    if (!$(this).isActive() && !$(this).isDisable())
                        $(this).isHover(true);
                })
                .on("mouseleave", function () {
                    $(this).isHover(false);
                });

            if (opts.collapsed) {
                $(">h3", el).prepend($("<span/>").addClass("icon d-icon-plus-2"));
            } else {
                $(">h3", el).prepend($("<span/>").addClass("icon " + (opts.active ? "d-icon-minus-2" : "d-icon-plus-2")))
                .eq(opts.active)
                .isActive(true);
            }

            if ($(">h3.d-state-active>.icon", el).length)
                $(">h3.d-state-active>.icon", el).removeClass("d-icon-plus-2")
                                                                     .addClass("d-icon-minus-2");
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("active") != undefined) opts.active = el.data("active");
            if (el.data("collapsed") != undefined) opts.collapsed = el.dataBool("collapsed");
        },
        _setActiveIndex: function (index) {
            return this._setActive($(">h3", el).eq(index));
        },
        _setActive: function (section) {
            var activeCls = "d-state-active", el = this.element,
                opts = this.options, section_content = section.next();

            if (section.children(".d-icon-loading").length)
                return el;

            var _do = function () {
                if (!opts.collapsible) {
                    $(">div.d-state-active", el).not(section_content)
                                                                .animate({ height: "toggle" }, opts.duration, function () {
                                                                    $(this).removeClass(activeCls);
                                                                    $(">.icon", $(this).prev()).removeClass("d-icon-minus-2").addClass("d-icon-plus-2");
                                                                });

                    $(">h3.d-state-active", el).not(section).removeClass(activeCls);
                }

                section.toggleClass(activeCls)
                          .next()
                          .animate({ height: "toggle" }, opts.duration)
                          .toggleClass(activeCls);

                $(">.icon", section).toggleClass("d-icon-plus-2").toggleClass("d-icon-minus-2");
            };

            if (section_content.data("url")) {
                section.children(".icon").addClass("d-icon-loading");
                section_content.load(section_content.data("url"), function () {

                    section.children(".icon")
                                .removeClass("d-icon-loading");

                    section_content.data("height", "auto")
                                               .removeAttr("data-url")
                                               .taoUI();
                    _do();
                });
            } else {
                _do();
            }

            return el;
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
            if (key == "acitve")
                this._setActiveIndex(value);
            return $.Widget.prototype._setOption.call(this, key, value);
        },
        destroy: function () {
            this.element.removeClass("d-accordion");
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);