(function ($) {
    $.widget("dna.taoForm", {
        options: {
            datasource: null,
            mode: "display", // display | edit | new,
            cancel: null,
            deleted: null,
            saved: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();

            if (opts.datasource)
                this._setDataSource(opts.datasource);

        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options,self=this;
            if (el.data("source")) {
                opts.datasource = el.datajQuery("source");
                var inputs = $("[name]", el);
                inputs.each(function (i, input) {
                    var _input = $(input);
                    if (_input.data("bind") == undefined) {
                        _input.attr("data-bind", el.attr("data-source"));
                    }
                });
            }

            if (el.data("mode"))
                opts.mode = el.data("mode");

            if (opts.datasource.length) {
                if (!opts.fields) opts.fields = [];
                var schema = opts.datasource.taoDataSource("option", "schema");
                if (schema.fields) {
                    opts.fields = $.makeArray(schema.fields);
                    $.each(opts.fields, function (i, f) {
                        f.showLabel = true;
                        if (f.type == "datetime")
                            f.widget = "datetime";
                        else
                            f.widget = "textbox";
                    });
                }
            }

            $("[data-rel='update']",el).unbind("click")
                                                  .click(function () {
                                                      self.update();
                                                  });

            $("[data-rel='delete']",el).unbind("click")
                                      .click(function () {
                                          self._source.taoDataSource("remove");
                                      });
            return this;
        },
        _setDataSource: function (val) {
            var self = this, opts = this.options;
            if (val) {
                //Binding to datasource widget
                if ($.isPlainObject(val)) {
                    throw "Could not bind to none datasource object."
                } else {
                    if (!val.jquery) throw "The input object is not a valid datasource object";
                    var evtPrefix = "taoDataSource";

                    if (this._source) {
                        this._source.unbind(evtPrefix + "position", $.proxy(this._onDataPosition, this))
                                         .unbind(evtPrefix + "inserted", $.proxy(this._onInserted, this))
                                         .unbind(evtPrefix + "updated", $.proxy(this._onUpdated, this))
                                         .unbind(evtPrefix + "removed", $.proxy(this._onRemoved, this))
                                         .bindunbind(evtPrefix + "process", $.proxy(this._onProcess, this))
                                         .unbind(evtPrefix + "error", $.proxy(this._onError, this))
                                         .unbind(evtPrefix + "completed", $.proxy(this._onCompleted, this));
                    }

                    this._source = val;

                    this._source.bind(evtPrefix + "position", $.proxy(this._onDataPosition, this))
                                         .bind(evtPrefix + "inserted", $.proxy(this._onInserted, this))
                                         .bind(evtPrefix + "updated", $.proxy(this._onUpdated, this))
                                         .bind(evtPrefix + "removed", $.proxy(this._onRemoved, this))
                                         .bind(evtPrefix + "process", $.proxy(this._onProcess, this))
                                         .bind(evtPrefix + "error", $.proxy(this._onError, this))
                                         .bind(evtPrefix + "completed", $.proxy(this._onCompleted, this));
                }
            }
        },
        _onDataPosition: function (event, dataItem) {
            this._currentData = dataItem;
            //The elements not support databind
            $("[name][type=hidden],textarea[name]",this.element).each(function (i,itemEle) {
                var _name = $(itemEle).attr("name");
                $(itemEle).val(dataItem[_name]);
            });
        },
        _onInserted: function (event, data) {
            $("[name][type=hidden],textarea[name]", this.element).each(function (i, itemEle) {
                var _name = $(itemEle).attr("name");
                $(itemEle).val("");
            });
            this.element.isDisable(false);
        },
        _onUpdated: function (event, data) {
            //this._triggerEvent("saved");
        },
        _onRemoved: function (event, data) {
            var _data = this._source.taoDataSource('option', 'data');
            if (_data == null || _data == undefined || _data.length == 0) {
                this.element.isDisable(true);
            }
        },
        _onProcess: function (event, data) {
            //this.element.blockUI();
        },
        _onError: function (event, data) {
        },
        _onCompleted: function () {
            //  this.element.unblockUI();
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        update: function () {
            var dataItem=this._getData(),opts=this.options;
            //console.log(opts.mode);
            if (opts.mode=="new")
                this._source.taoDataSource("insert", dataItem);
            if (opts.mode == "edit")
                this._source.taoDataSource("update", dataItem);
        },
        _getData: function () {
            var obj = {}, el = this.element;
            
            var inputs = $("[name]", el);
            inputs.each(function (i, inputEle) {
                obj[$(inputEle).attr("name")] = $(inputEle).val();
            });
            return obj;
        },
        reset: function () {
            var inputs = $("[name]", this.element);
            inputs.each(function (i, input) {
                var _input = $(input);
                _input.val("");
            });
            this.element.isDisable(false);
        },
        disable: function () {
            this.widget().addClass("d-state-disable");
            return this;
        },
        enable: function () {
            this.widget().removeClass("d-state-disable");
            return this;
        },
        _setOption: function (key, value) {
            if (key == "datasource") {
                this._setDataSource(value);
                return this;
            }
            if (key == "mode") {
                this.options.mode = value;
                if (value == "new") {
                    this.reset();
                    $("[data-rel='delete']", this.element).hide();
                } else {
                    $("[data-rel='delete']", this.element).show();
                }
            }

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);