(function ($) {
    $.widget("dna.taoListbox", $.dna.taoListview, {
        options: {
            value: "",
            dataValueField: "value",
            dataTextField: "label",
            valueTo: null,
            checkable: false,
            itemsAnimation: false,
            highlightfirst: true,
            selectable: true,
            hasFocus: true,
            check: null,
            checkValTo: null,
            checkValues: null,
            sortable: false
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix, _tag = el[0].tagName.toLowerCase();

            this.initialized = false;
            this._unobtrusive();

            if (_tag == "input" || _tag == "select") {
                //For <input> and <select>
                var wrapper = $("<ul/>").addClass("d-reset d-ui-widget d-items d-listbox");
                el.after(wrapper);

                //clone attributes 
                if (el.attr("style")) wrapper.attr("style", el.attr("style"));
                if (el.attr("class")) wrapper.addClass(el.attr("class"));

                this._container = wrapper;
                this._valueHolder = el;
                el.hide();

                if (_tag == "select") {
                    if (opts.datasource == null) {
                        opts.datasource = {
                            mapper: new tao.htmlSelectMapper(),
                            data: el
                        }
                    }
                }
            } else {
                this._valueHolder = $("<input type='hidden' />");
                if (_tag == "ul") {
                    //For <ul>
                    el.addClass("d-ui-widget d-items d-listbox");
                    this._container = el;
                    el.after(this._valueHolder);
                }
                else {
                    //For <div>
                    this._container = $("<ul/>").addClass("d-ui-widget d-items d-listbox")
                                                                .appendTo(el);
                    el.append(this._valueHolder);
                }
            }

            if (opts.check)
                el.bind(eventPrefix + "check", opts.check);

            this._bindEvents()
                  ._getItemsContainer()
                  .addClass("d-items");

            if (opts.scroller) {
                if (opts.scroller == "self") {
                    this._scroller = $("<div />").attr("style", el.attr("style")).css({ "overflow": "auto" }).addClass("d-listbox");
                    el.after(this._scroller);
                    el.appendTo(this._scroller);
                    el.removeAttr("style").removeClass("d-listbox");
                }
                this._setScroller(opts.scroller);
            }

            if (opts.checkable)
                el.addClass("d-checkable");

            if (opts.datasource)
                this._setDataSource(opts.datasource);
            else
                this._createDataElements(); //Init element without data source.

            if (opts.checkable) {
                if (opts.checkValues) {
                    self._setCheckValues(opts.checkValues);
                    if (opts.checkValTo) self._fillCheckValues();
                }
            }

            if (opts.sortable) {
                wrapper.sortable({
                    update: function () {
                        self._fillCheckValues();
                    }
                });
            }

            if (opts.value) {
                this._valueHolder.val(opts.value);
            } else {
                if (this._valueHolder.val()) {
                    opts.value = this._valueHolder.val();
                }
            }

            if (opts.value) {
                if ($.type(opts.value) == "string" && opts.value.indexOf(",") > -1 && opts.selectable == "multi")
                    this.select(opts.value.split(","));
                else
                    this.select(opts.value);
            }
        },
        find: function (val) {
            ///<summary>Find item by value</summary>
            var _items = $(">.d-item", this._getItemsContainer()), _targetItem = null, opts = this.options;
            if (opts.datasource) {
                for (var i = 0; i < _items.length; i++) {
                    var _di = $(_items[i]).data("dataItem");
                    if (_di && opts.dataValueField) {
                        var _v = $.isPlainObject(_di) ? _di[opts.dataValueField] : _di;
                        if (_v == val) 
                            return $(_items[i]);
                    }
                }

                return _targetItem;
            }
            else
                return $(">.d-item[data-" + opts.dataValueField + "=" + val + "]", this._getItemsContainer());
        },
        select: function (values) {
            var self = this;
            this.clearSelection();
            if (values) {
                if ($.isArray(values)) {  //Multi Select
                    var resultItems = [];
                    $.each(values, function (i, v) {
                        var t = self.find(v);
                        if (t && t.length) {
                            self._setItemSelected(t);
                            resultItems.push(t);
                        }
                    });

                    if (resultItems.length)
                        this.options.value = values.join(",");
                    else
                        this.options.value = "";

                    this._getValueHolder().val(this.options.value);
                    return resultItems;
                }
                else {
                    var resultItem = this.find(values);
                    if (resultItem && resultItem.length) {
                        this._setItemSelected(resultItem);
                        this.options.value = values;
                        this._getValueHolder().val(values);
                        return resultItem;
                    }
                }
            }
            return null;
        },
        clearSelection: function () {
            this.options.value = "";
            return $.dna.taoListview.prototype.clearSelection.call(this);
        },
        selectedText: function () {
            var dataItems = this.selectedDataItems();
            if (dataItems && dataItems.length)
                return dataItems[0][this.options.dataTextField];
            return "";
        },
        selectedDataItems: function () {
            var data = [], items = this.getSelectedItems(), opts = this.options;
            items.each(function (i, item) {
                if (opts.datasource)
                    data.push($(item).data("dataItem")); else {
                    var obj = {};
                    obj[opts.dataValueField] = $(item).attr("data-" + opts.dataValueField);
                    obj[opts.dataTextField] = $(item).attr("data-" + opts.dataTextField);
                    data.push(obj);
                }
            });

            return data;
        },
        checkItems: function () {
            var self = this, el = this.element, container = this._getItemsContainer();
            var items = container.find(".d-checkbox.d-state-active");
            var results = [];
            items.each(function (i, n) {
                results.push($(n).parent());
            });
            return results;
        },
        checkDataItems: function () {
            var items = this.checkItems(), dataArgs = [];
            $.each(items, function (i, n) {
                dataArgs.push($(n).data("dataItem"));
            });
            return dataArgs;
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;
            $.dna.taoListview.prototype._unobtrusive.call(this, element);

            if (el.data("valuefield")) opts.dataValueField = el.data("valuefield");
            if (el.data("textfield")) opts.dataTextField = el.data("textfield");

            if (el.data("checkable") != undefined) opts.checkable = el.dataBool("checkable");
            if (el.data("sortable") != undefined) opts.sortable = el.dataBool("sortable");
            if (el.data("check")) opts.check = new Function("event", "ui", el.data("check"));
            if (el.data("valto")) opts.valueTo = el.datajQuery("valto");

            if (el.data("checkto")) opts.checkValTo = el.datajQuery("checkto");
            if (el.data("check-vals"))
                opts.checkValues = el.data("check-vals").toString().split(",");
            return this;
        },
        getValHolder: function () {
            return this._valueHolder;
        },
        _getValueHolder: function () {
            return $(this._valueHolder);
        },
        _onItemCreated: function (element, data) {
            var self = this;
            if (data) {
                ///TODO:Add group item support.
                if ($.isPlainObject(data)) {
                    element.data("value", data[this.options.dataValueField]);

                    if (!this.options.itemTmpl && !element.children().length) // no tmpl
                        element.text(data[this.options.dataTextField])
                }
                else {
                    element.data("value", data);
                    if (!this.options.itemTmpl && !element.children().length) // no tmpl
                        element.text(data);
                }
            }

            if (this.options.checkable) {
                $(element).wrapInner("<a/>");
                $(element).children("a").disableSelection();
                var checkbox = $("<input type='checkbox'/>");
                checkbox.bind("taoCheckboxchange", function (event, ui) {
                    data.selected = ui.checked;
                    self._triggerEvent("check", { item: $(element), dataItem: data });
                    self._fillCheckValues();
                });

                if (data && data.selected)
                    checkbox.attr("checked", "checked");

                $(element).prepend(checkbox);
            }

            $.dna.taoListview.prototype._onItemCreated.call(this, element, data);

            if (data && data.selected)
                this._setItemSelected(element);

            return this;
        },
        _onItemClick: function (element) {
            $.dna.taoListview.prototype._onItemClick.call(this, element);

            if (this.element[0].tagName.toLowerCase() == "select" || this.element[0].tagName.toLowerCase() == "input")
                this.element.trigger("change");

            if (this.options.checkable) {
                var _checkbox = $("input", element),
                    _fire = _checkbox.attr("checked") != "checked";
                _checkbox.taoCheckbox("check", true);
                var data = element.data("data") ? element.data("data") : element.data("value");

                if (_fire) {
                    this._fillCheckValues();
                    this._triggerEvent("check", { item: element, dataItem: data });
                }
            }

            return this.element;
        },
        _onItemSelected: function (index, item) {
            //var _val = item.data("value");
            //this._getValueHolder().val(_val);
            var _dataItem = item.data("dataItem"), opts = this.options,
                valField = opts.dataValueField,
                txtField = opts.dataTextField,
                    txt = "";

            if (_dataItem) {
                opts.value = _dataItem[valField];
                txt = _dataItem[txtField];
                this.getValHolder().val(opts.value);
            } else {
                opts.value = item.attr("data-" + valField);
                if (txtField)
                    txt = item.attr("data-" + txtField);
                else
                    txt = item.text();
            }

            if (this.options.valueTo) {
                var _alt = $(this.options.valueTo);
                if (_alt.length) {
                    if (_alt.isInput())
                        _alt.val(this.options.value);
                    else
                        _alt.text(this.options.value);
                }
            }

            //if (item.isActive()) {
            this._triggerEvent("select", { index: index, item: item, dataItem: _dataItem, value: opts.value, text: txt });
            //} else {
            //    if (!item.isDisable()) {
            //        this._triggerEvent("cancel", { index: index, item: item, dataItem: _dataItem, value: opts.value, text: txt });
            //    }
            //}
            //$.dna.taoListview.prototype._onItemSelected.call(this, index, item);
            //this._triggerEvent("select", { index: index, item: item });
        },
        _getCheckValues: function () {
            var self = this, opts = this.options;
            //Fill check values to value holder
            var _dataArgs = self.checkDataItems(), _vals = [];
            if (_dataArgs.length) {
                $.each(_dataArgs, function (i, n) {
                    if ($.isPlainObject(n))
                        _vals.push(n[self.options.dataValueField]);
                    else
                        _vals.push(n.toString());
                });
            }
            return _vals;
        },
        _setCheckValues: function (vals) {
            var self = this, opts = this.options, container = this._getItemsContainer();
            if (opts.checkable) {
                container.children().each(function (i, n) {
                    var checkbox = $(n).find("input[type=checkbox]"),
                        data = $(n).data("data") ? $(n).data("data") : $(n).data("value"), itemVal = null;

                    if ($.isPlainObject(data))
                        itemVal = data[opts.dataValueField];
                    else
                        itemVal = data;

                    if ($.inArray(itemVal, vals) > -1)
                        checkbox.taoCheckbox("check", true);
                    else
                        checkbox.taoCheckbox("check", false);
                });
            }
        },
        _fillCheckValues: function () {
            var self = this, opts = this.options, vals = this._getCheckValues(), checkValHolder = opts.checkValTo;
            //Fill check values to value holder
            if (checkValHolder && opts.checkable) {
                if (vals.length) {
                    checkValHolder.val(vals.join(","));
                } else {
                    checkValHolder.val("");
                }
            }
        },
        _setOption: function (key, value) {
            if (key == "checkValues") {
                this._setCheckValues(value);
                this.options.checkValues = value;
            }

            if (key == "value")
                this.select(value);

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        widget: function () {
            return this._container;
        },
        _getItemsContainer: function () {
            return this._container;
        }
    });
})(jQuery);