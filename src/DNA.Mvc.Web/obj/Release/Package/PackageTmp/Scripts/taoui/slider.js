(function ($) {
    $.widget("dna.taoSlider", {
        options: {
            step: 1,
            value: 0,
            max: 100,
            min: 0,
            orientation: "horizontal",
            to: null,
            slide: null,
            start: null,
            stop: null,
            change: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix,
                el = this.element, sliderEle = el;
            this._unobtrusive();

            ///TODO: The orientation,min and step is not use now.

            if (el[0].tagName == "INPUT") {
                el.wrap("<div/>");
                sliderEle = el.parent();
            }

            if (opts.slide)
                el.bind(eventPrefix + "slide", opts.slide);

            if (opts.start)
                el.bind(eventPrefix + "start", opts.start);

            if (opts.stop)
                el.bind(eventPrefix + "stop", opts.stop);

            if (opts.change)
                el.bind(eventPrefix + "change", opts.change);

            sliderEle.addClass("d-reset d-ui-widget d-slider");
            var handler = $("<div/>").addClass("d-ui-widget d-slider-handler")
                               .appendTo(sliderEle)
                               .hover(function () { $(this).isHover(true); }, function () { $(this).isHover(false); }),
            stepWidth = sliderEle.width() / opts.max,
            valEle = $("<div/>").addClass("d-slider-value d-ui-widget-content d-state-active")
                                            .appendTo(sliderEle);
            var helper = null,
                _value = opts.value;

            if (handler.isVisible()) {
                handler.position({
                    of: sliderEle,
                    my: "center center",
                    at: "left center"
                });
            }

            if (opts.value) {
                var p = ((opts.value / opts.max) * sliderEle.width()) + "px";
                if (sliderEle.width() == 0) 
                    p = ((opts.value / opts.max)*100) + "%";
                valEle.css("width", p);
                handler.css("left", p);
            } else {
                handler.css("left", "0px");
            }

            handler.draggable({
                axis: opts.orientation == "horizontal" ? "x" : "y",
                containment: "parent",
                drag: function (event, ui) {
                    valEle.css("width", ui.position.left + "px");
                    var val = valEle.width() / sliderEle.width(),
                        present = val >= 0.99 ? 1 : val;

                    _value = Math.round(present * opts.max);

                    helper.css("z-index", $.topMostIndex())
                              .position({
                                  of: handler,
                                  at: "middle top",
                                  my: "middle bottom",
                                  offset: "0px -10px"
                              })
                             .text(_value);

                    if (opts.to)
                        opts.to.val(_value);

                    var eventArgs = { value: _value };
                    self._triggerEvent("slide", eventArgs);
                },
                stop: function (event, ui) {
                    self._triggerEvent("stop", { value: _value });
                    handler.isActive(false);
                    if (opts.value != _value) {
                        if (el[0].tagName == "INPUT") {
                            el.val(_value);
                            el.trigger("change");
                        }
                        self._triggerEvent("change", { value: _value });
                    }
                    if (helper)
                        helper.remove();
                },
                start: function (event, ui) {
                    handler.isActive(true);
                    self._triggerEvent("start");
                }
            }).bind("mousedown", function () {
                helper = $("<div/>").addClass("d-slider-helper d-ui-widget-content").appendTo(sliderEle);
                helper.text(opts.value)
                    .css("z-index", $.topMostIndex())
                    .position({
                        of: handler,
                        at: "middle top",
                        my: "middle bottom",
                        offset: "0px -10px"
                    });
            })
            .bind("mouseup", function () {
                if (helper)
                    helper.remove();
            });

            return el;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("max") != undefined) opts.max = parseFloat(el.data("max"));
            if (el.data("min") != undefined) opts.min = parseFloat(el.data("min"));
            if (el.data("orientation")) opts.orientation = el.data("orientation");
            //            if (el.data("range") != undefined) opts.range = el.data("range");
            if (el.data("step") != undefined) opts.step = el.data("step");
            if (el.data("value") != undefined)
                opts.value = el.dataInt("value");
            else
            {
                if (el.val() != "")
                    opts.value = el.val();
            }

            if (el.data("to") != undefined) opts.to = el.datajQuery("to");
            if (el.data("start")) opts.start = new Function("event", "ui", el.data("start"));
            if (el.data("slide")) opts.slide = new Function("event", "ui", el.data("slide"));
            if (el.data("change")) opts.change = new Function("event", "ui", el.data("change"));
            if (el.data("stop")) opts.stop = new Function("event", "ui", el.data("stop"));
        },
        _setVal: function (val) {
            var el = this.element, opts = this.options;
            opts.value = val;
            var sliderEle = el;

            if (el[0].tagName == "INPUT") {
                el.val(val);
                sliderEle = el.parent();
            }

            if (opts.to)
                opts.to.val(val);

            var p = ((opts.value / opts.max) * sliderEle.width()) + "px";
            $(".d-slider-value", sliderEle).css("width", p);
            $(".d-slider-handler", sliderEle).css("left", p)

            return this.element;
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
            if (key == "value")
                this._setVal(value);

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);