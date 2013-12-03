(function ($) {
    $.widget("dna.taoTags", $.dna.taoDataBindable, {
        options: {
            value: null,
            dataValueField: "value",
            dataTextField: "text",
            dataTooltipField: "tooltip",
            added: null,
            removed: null,
            itemCreated: null,
            itemClick: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element,
            wrapper = $("<ul/>").addClass("d-reset d-tags");
            this.wrapper = wrapper;

            $.dna.taoDataBindable.prototype._create.call(this);
            el.before(wrapper);
            this.val(opts.value ? opts.value : el.val());
            if (el.attr("style"))
                wrapper.attr("style", el.attr("style"));

            if (el.hasClass("d-inline") || el.attr("data-inline") == "true")
              wrapper.addClass("d-inline");

            el.hide();

            if (opts.itemCreated)
                el.bind(eventPrefix + "itemCreated", opts.itemCreated);

            if (opts.added)
                el.bind(eventPrefix + "added", opts.added);

            if (opts.removed)
                el.bind(eventPrefix + "removed", opts.removed);

            if (opts.itemClick)
                el.bind(eventPrefix + "itemClick", opts.itemClick);
        },
        _unobtrusive: function () {
            var el = this.element, self = this, opts = this.options;
            if (el.data("valuefield")) opts.dataValueField = el.data("valuefield");
            if (el.data("textfield")) opts.dataTextField = el.data("textfield");
            if (el.data("tooltipfield")) opts.dataTooltipField = el.data("tooltipfield");
            if (el.data("onitemcreated")) opts.itemCreated = new Function("event", "ui", el.data("onitemcreated"));
            if (el.data("onitemclick")) opts.itemClick = new Function("event", "value", el.data("onitemclick"));
            if (el.data("added")) opts.added = new Function("event", "ui", el.data("added"));
            if (el.data("removed")) opts.removed = new Function("event", "ui", el.data("removed"));
            $.dna.taoDataBindable.prototype._unobtrusive.call(this);
        },
        _tagsToVal: function () {
            var _tags = [];

            this.wrapper.children().each(function (i, n) {
                _tags.push($(n).data("value"));
            });

            if (_tags)
                this.element.val(_tags.join(","));
            else
                this.element.val("");

            this.options.value = this.element.val();
        },
        val: function (_tags) {
            var self = this, opts = this.options;
            if (_tags == undefined) {
                return this.element.val();
            }
            else {
                this.wrapper.empty();

                if ($.type(_tags) == "string") {
                    if (_tags) {
                        var labels = _tags.split(",");
                        $.each(labels, function (i, label) {
                            self._add(label);
                        });
                    }
                    if (this.element.val() != _tags)
                        this.element.val(_tags);
                }
                else {

                    $.each(_tags, function (i, _tag) {
                        self._add(_tag);
                    });

                    var _actualVals = [];

                    $.each(_tags, function (i, n) {
                        if (opts.dataValueField) {
                            var _av = n[opts.dataValueField];
                            if (_av != undefined)
                                _actualVals.push(_av);
                        }
                    });

                    this.element.val(_actualVals.join(","));

                }
                this.options.value = _tags;
            }
            return this;
        },
        contains: function (_tag) {
            var _tags = [], opts = this.options, _val = null;
            this.wrapper.children().each(function (i, n) {
                _tags.push($(n).data("value"));
            });

            if ($.isPlainObject(_tag)) {
                if (opts.dataValueField)
                    _val = _tag[opts.dataValueField];
            } else
                _val = _tag;


            if (_val == undefined || _val == null)
                return false;

            if ($.inArray(_val, _tags) > -1)
                return true;

            return false;
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
        _add: function (vTag) {
            if (!vTag)
                return null;

            var self = this, el = this.element, _tags = [], opts = this.options, _val, _label, _tooltip;
            this.wrapper.children().each(function (i, n) {
                _tags.push($(n).data("value"));
            });

            if ($.isPlainObject(vTag)) {
                if (opts.dataValueField)
                    _val = vTag[opts.dataValueField];

                if (opts.dataTextField)
                    _label = vTag[opts.dataTextField];

                if (opts.dataTooltipField)
                    _tooltip = vTag[opts.dataTooltipField];

            } else
                _val = _label = vTag;


            if (_val == undefined || _val == null)
                return null;

            if ($.inArray(_val, _tags) > -1)
                return null;

            var item = $("<li/>").addClass("d-tag d-button d-ui-widget")
                           .hover(function () { $(this).isHover(true); }, function () { $(this).isHover(false); })
                           .click(function () {
                               self._triggerEvent("itemClick", $(this).data("value"));
                           })
                           .append($("<span/>").addClass("d-button-text d-tag-label").text(_label))
                           .append($("<span/>").addClass("d-icon-cross-3")
                                                              .click(function () {
                                                                  var val = $(this).parent().data("value");
                                                                  $(this).parent().remove();
                                                                  self._tagsToVal();
                                                                  self._triggerEvent("removed", val);

                                                              })
                                         )
                            .appendTo(self.wrapper)
                            .data("value", _val);

            if (opts.mode == "display" || el.isReadonly())
                $(".d-icon-cross-3", item).remove();


            if (_tooltip)
                item.attr("title", _tooltip)
                       .attr("data-tooltip-position", "top")
                       .attr("data-tooltip-width", 150)
                       .taoTooltip();
            _tags.push(_val);
            this.element.val(_tags.join(","));

            if (item != null)
                this._triggerEvent("itemCreated", { item: item, dataItem: vTag });

            return item;
        },
        add: function (vTag) {
            var _item = this._add(vTag);
            if (_item != null)
                this._triggerEvent("added", { item: _item, dataItem: vTag });
            return this;
        },
        remove: function (vTag) {
            if (!vTag)
                return this;
            var self = this, _tags = [], opts = this.options, _val, _label, _tooltip;
            this.wrapper.children().each(function (i, n) {
                _tags.push($(n).data("value"));
            });

            if ($.isPlainObject(vTag)) {
                if (opts.dataValueField)
                    _val = vTag[opts.dataValueField];
            } else
                _val = _label = vTag;

            if (_val == undefined || _val == null)
                return this;

            if ($.inArray(_val, _tags) < 0)
                return this;

            var items = $("li", this.wrapper);
            items.each(function (i, n) {
                if ($(n).data("value") == _val) {
                    $(n).remove();
                    return;
                }
            });
            this.element.val(_tags.join(","));
            return this;
        },
        destroy: function () {
            if (this.wrapper)
                this.wrapper.remove();
            this.element.show();
            $.dna.taoDataBindable.prototype.destroy.call(this);
        }
    });
})(jQuery);