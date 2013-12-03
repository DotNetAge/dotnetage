(function ($) {
    $.widget("dna.taoSwitcher", $.dna.taoDataBindable, {
        options: {
            value: null,
            textA: "On",
            textB: "Off",
            valA: true,
            valB: false,
            changed: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix,
            el = this.element;
            $.dna.taoDataBindable.prototype._create.call(this);

            if (opts.changed)
                el.bind(eventPrefix + "changed", opts.changed);

            el.wrap("<div/>");
            this.on = false;
            var wrapper = el.parent();
            wrapper.addClass("d-switcher");
            if (el.attr("style"))
                wrapper.attr("style", el.attr("style"));

            if (el[0].tagName.toLowerCase() == "select") {
                if (el.children().length < 2) {
                    throw "The select element must be has two option elements at less.";
                    return el.hide();
                }

                var optsA = $(el.children()[0]), optsB = $(el.children()[1]);
                opts.textA = optsA.text();
                opts.valA = optsA.attr("value");
                opts.textB = optsB.text();
                opts.valB = optsB.attr("value");
            }

            var itemA = { text: opts.textA, value: opts.valA }, itemB = { text: opts.textB, value: opts.valB },
            labelA = $("<span/>").appendTo(wrapper)
                                .addClass("d-switcher-label a")
                                .text(itemA.text)
                                .click(function (e) {
                                    e.stopPropagation();
                                    self._toL();
                                })
                                .data("value", itemA.value),

            labelB = $("<span/>").appendTo(wrapper)
                                .addClass("d-switcher-label b")
                                .text(itemB.text)
                                .data("value", itemB.value)
                                .click(function (e) {
                                    e.stopPropagation();
                                    self._toR();
                                });

            this.labelA = labelA;
            this.labelB = labelB;
            this.wrapper = wrapper;
            this.switcher = $("<div/>").appendTo(wrapper)
                                                      .addClass("d-switcher-slider d-tran")
                                                      .draggable({
                                                          axis: "x",
                                                          containment: "parent",
                                                          scrollSpeed: 40,
                                                          stop: function (evt, ui) {
                                                              var cx = (ui.helper.width() / 2) + ui.position.left, hw = wrapper.width() / 2;
                                                              if (hw > cx)
                                                                  self._toL();
                                                              else
                                                                  self._toR();
                                                          }
                                                      });

            if (opts.value != undefined && opts.value != null)
                el.val(opts.value);
            else
                opts.value = el.val();


            if (el.val().toString() == itemB.value.toString()) {
                this.switcher.removeAttr("style");
                labelA.css({ width: "0px" });
                labelB.css({ width: "100%" });
                this.switcher.css({ "left": "0" });
                this.on = false;
            }

            el.hide();
        },
        _toL: function () {
            this.switcher.removeAttr("style");
            this.switcher.stop().animate({ "left": "0" }, 40);
            this.labelB.animate({ "width": "100%" }, 50);
            this.labelA.animate({ "width": "0" }, 50);
            this.element.val(this.labelB.data("value"));
            this.on = false;
            this._triggerEvent("changed", this.labelB.data("value"));
            return this;
        },
        _toR: function () {
            this.switcher.removeAttr("style");
            this.switcher.stop().animate({ "right": "0" }, 40);
            this.labelA.animate({ "width": "100%" }, 50);
            this.labelB.animate({ "width": "0" }, 50);
            this.element.val(this.labelA.data("value"));
            this.on = true;
            this._triggerEvent("changed", this.labelA.data("value"));
            return this;
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("text-a")) opts.textA = el.data("text-a");
            if (el.data("text-b")) opts.textB = el.data("text-b");
            if (el.data("val-a")) opts.valA = el.data("val-a");
            if (el.data("val-b")) opts.valB = el.data("val-b");
            if (el.data("onchanged")) opts.changed = new Function("event", "value", el.data("onchanged"));
            return $.dna.taoDataBindable.prototype._unobtrusive.call(this);
        },
        toggle: function () {
            if (this.on)
                this._toL();
            else
                this._toR();
            return this;
        },
        val: function (_val) {
            if (_val) {
                if (this.labelA.data("value") == val)
                    this._toL();
                else
                    this._toR();
                return this;
            }
            else
                return this.element.val();
        },
        _setOption: function (key, value) {
            if (key == "value") {
                this.val(value);
                return this;
            }
            return $.dna.taoDataBindable.prototype._setOption.call(this, key, value);
        },
        _onBindingPosition: function (data) {
            if (data) {
                if (data.value != undefined)
                    this.val(data.value);
            }
            return this;
        },
        destroy: function () {
            if (this.wrapper) {
                this.element.before(this.wrapper);
                this.wrapper.remove();
            }
            this.element.show();
            return $.dna.taoDataBindable.prototype.destroy.call(this, key, value);
        }
    });
})(jQuery);