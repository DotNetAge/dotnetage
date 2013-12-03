(function ($) {
    $.widget("dna.taoListview", $.dna.taoDataBindingList, {
        options: {
            highlightfirst: false,
            itemClass: null,
            hoverClass: null,
            activeClass: null,
            selectable: false, //possible values : true | false | multi
            select: null,
            cancel: null,  //cancel selection (only available for selectable set to "multi")
            //itemClick: null,
            autoSelect: false,
            sortable: false,
            autoBlocking: false,
            itemsAnimation: false,
            hasFocus: false,
            inline: false,
            unselectOnClick: false,
            itemStyle: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive()
                    ._bindEvents()
                   ._getItemsContainer()
                   .addClass("d-items");

            //if (opts.inset)
            //    this._getItemsContainer().addClass("d-inset-list");

            if (opts.scroller)
                this._setScroller(opts.scroller);

            if (opts.sortable)
                this._setSortable(opts.sortable);

            if (opts.datasource)
                this._setDataSource(opts.datasource);
            else
                this._createDataElements(); //Init element without data source.

            el.addClass("d-reset d-listview");
        },
        _bindEvents: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            if (opts.select)
                el.bind(eventPrefix + "select", opts.select);

            if (opts.cancel)
                el.bind(eventPrefix + "cancel", opts.cancel);

            if (opts.itemCreated)
                el.bind(eventPrefix + "itemCreated", opts.itemCreated);

            if (opts.itemRemoved)
                el.bind(eventPrefix + "itemRemoved", opts.itemRemoved);

            if (opts.itemsAdded)
                el.bind(eventPrefix + "itemsAdded", opts.itemsAdded);
            return this;
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;
            $.dna.taoDataBindingList.prototype._unobtrusive.call(this, element);

            if (el.data("selectable") != undefined)
                opts.selectable = el.data("selectable");

            if (el.data("select")) opts.select = new Function("event", "ui", el.data("select"));
            if (el.data("cancel")) opts.cancel = new Function("event", "ui", el.data("cancel"));

            if (el.data("autohighlight") != undefined) opts.highlightfirst = el.dataBool("autohighlight");
            if (el.data("sortable") != undefined) opts.sortable = el.dataBool("sortable");
            if (el.data("unselect-click") != undefined) opts.unselectOnClick = el.dataBool("unselect-click");
            if (el.data("autoselect") != undefined) opts.autoSelect = el.dataBool("autoselect");
            if (el.data("autoblocking") != undefined) opts.autoBlocking = el.dataBool("autoblocking");
            //if (el.data("inset") != undefined) opts.inset = el.dataBool("inset");

            if (el.data("item-class")) opts.itemClass = el.data("item-class");
            if (el.data("active-class")) opts.activeClass = el.data("active-class");
            if (el.data("hover-class")) opts.hoverClass = el.data("hover-class");

            if (el.data("hasfocus") != undefined) opts.hasFocus = el.dataBool("hasfocus");
            if (el.data("inline") != undefined) opts.inline = el.dataBool("inline");
            if (el.data("item-style")) opts.itemStyle = el.data("item-style");
            return this;
        },
        _enableHighlightFirst: function () {
            if (this.options.highlightfirst) {
                if ($(">li.d-state-active", this._getItemsContainer()).length == 0) {
                    var firstitem = $(">li:first", this._getItemsContainer());
                    if (firstitem.length)
                        this._setItemSelected(firstitem);
                    //this._onItemClick(firstitem);
                }
            }
            return this;
        },
        _onItemSelected: function (index, item) {
            //protect method
            var _dataItem = item.data("dataItem");

            if (item.isActive()) {
                this._triggerEvent("select", { index: index, item: item, dataItem: _dataItem });
            } else {
                if (!item.isDisable()) {
                    this._triggerEvent("cancel", { index: index, item: item, dataItem: _dataItem });
                }
            }

            if (this._source && _dataItem) {
                this._source.taoDataSource("pos", _dataItem);
            }
        },
        _onProcess: function () {
            //if (this.options.autoBlocking)
            //    this._getItemsContainer().parent().blockUI();
        },
        _onComplete: function () {
            //if (this.options.autoBlocking)
            //    this._getItemsContainer().parent().unblockUI();
        },
        _onItemClick: function (item) {
            if ($(item).data("role") == "fieldcontain" || $(item).dataBool("readonly"))
                return;

            this._setItemSelected(item);
            this._onItemSelected($(item).siblings().andSelf().index(item), item);
            $(item).focus();
        },
        _onItemHover: function (item) {
            if (this.options.autoSelect && this.options.selectable) {
                this._onItemSelected($(item).siblings().andSelf().index(item), item);
                this._setItemSelected(item);
            }
            this._onItemFocus(item);
        },
        _onItemUnhover: function (item) {
            this._onItemBlur(item);
        },
        _onItemBlur: function (item) {
            //if (this.options.selectable) {
            item.isHover(false);
            if (this.options.hoverClass)
                item.removeClass(this.options.hoverClass);
            //}
        },
        _onItemFocus: function (item) {
            var self = this, opts = this.options;

            if (!item.isDisable() && !item.isActive()) {
                if (self.options.autoSelect && this.options.selectable) {
                    self._onItemSelected($(item).siblings().andSelf().index(item), item);
                    self._setItemSelected(item);
                }
                else {
                    if ($(this).data("role") == "fieldcontain" || $(this).dataBool("readonly")) return;
                    item.isHover(true);
                    if (opts.hoverClass)
                        item.addClass(opts.hoverClass);
                }
            }

        },
        _onItemKeypress: function (event, item) {
            var self = this;
            if (event.keyCode == 40 || event.keyCode == 38) {
                var i = parseInt($(item).attr("tabIndex"));
                var sibling = $(item).siblings("[tabIndex='" + (i + (event.keyCode == 40 ? 1 : -1)) + "']");
                if (sibling.length)
                    sibling.focus();
            }
            else {
                if (event.keyCode == 13) {
                    self._onItemSelected($(item).siblings().andSelf().index(item), item);
                    self._setItemSelected(item);
                }
                else {
                    if (event.keyCode == 46)
                        self._onitemdelete(item);
                }
            }
        },
        _setSortable: function (val) {
            if (val && $.fn.sortable) {
                try {
                    $(this._getItemsContainer()).sortable({
                        forcePlaceholderSize: true,
                        forceHelperSize: true,
                        items: ">.d-item",
                        containment: "parent",
                        axis: "y"
                    });
                } catch (expr) { console.log(expr); }
            }
        },
        _getNextServerPage: function () {
            var self = this, opts = this.options;
            //load on demond
            if (opts.datasource && opts.datasource.jquery && $.fn.taoDataSource && opts.datasource.taoDataSource("option", "serverPaging")) {
                var src = opts.datasource;
                return src.taoDataSource("nextPage");
            }
            return $.Deferred().resolve();
        },
        _createDataElements: function (data) {
            if (data == undefined) {
                var self = this;
                $(">li", this._getItemsContainer()).each(function (i, n) {
                    self._onItemCreated(n);
                });
            }
            else
                $.dna.taoDataBindingList.prototype._createDataElements.call(this, data);

            if (this.options.hasFocus)
                this._setTabIndexs()

            return this._enableHighlightFirst();
        },
        _onItemCreated: function (element, data) {
            var self = this, opts = this.options;
            if (!this.delay) this.delay = 0;
            var delay = this.delay + 10;

            // if ("list-divider" != $(element).data("role")) {
            $(element).addClass("d-item" + (this.options.inline ? " d-float-left" : "") + (this.options.itemClass ? " " + this.options.itemClass : ""))
                                .attr("role", "listitem")
                                .hover(function (e) {
                                    if ($(this).data("role") == "fieldcontain" || $(this).dataBool("readonly")) return;
                                    e.stopPropagation();
                                    e.preventDefault();
                                    self._onItemHover($(this));
                                }, function (e) {
                                    // if ($(this).data("role") == "fieldcontain" || $(this).dataBool("readonly")) return;
                                    e.stopPropagation();
                                    e.preventDefault();
                                    self._onItemUnhover($(this));
                                })
                                .bind("click", function (e) {
                                    //e.preventDefault();
                                    e.stopPropagation();
                                    self._onItemClick($(this));
                                    self._triggerEvent("itemclick", $(this));
                                })
                                .focus(function () { self._onItemFocus($(this)); })
                                .blur(function () { self._onItemBlur($(this)); })
                               .keypress(function (event) {
                                   if (opts.hasFocus) {
                                       event.stopPropagation();
                                       event.preventDefault();
                                       self._onItemKeypress(event, this);
                                   }
                               });

            if (opts.itemStyle) {
                var _style = $(element).attr("style") ? $(element).attr("style") : "";
                $(element).attr("style", opts.itemStyle + _style);
            }
            //} else {
            //    $(element).addClass("d-item-divider");
            //}

            if ($(element).isVisible()) {
                if (self.options.itemsAnimation) {
                    $(element).hide().delay(delay).fadeIn("normal");
                    this.delay = delay;
                }
            }

            //$(element).taoUI();
            return this;
        },
        _setTabIndexs: function () {
            var items = $(">li", this._getItemsContainer());
            items.each(function (i, n) {
                $(n).attr("tabIndex", i)
            });
            return this;
        },
        _setItemSelected: function (el) {
            var opts = this.options;
            if (!$(el).isDisable()) {
                this._setPosition(el);
                //change states
                if (opts.selectable != "multi") {
                    $(el).siblings(".d-state-active").removeClass("d-state-active");

                    if (opts.activeClass)
                        $(el).siblings(opts.activeClass).removeClass(opts.activeClass);
                }

                if (opts.selectable) {
                    //if (opts.selectable != "multi" && $(el).isActive())
                    //    $(el).isActive(false);
                    //else {
                    //if ($(el).isActive()) return;
                    //$(el).isActive(!$(el).isActive());
                    $(el).isActive(true);
                    if (opts.activeClass)
                        $(el).toggleClass(opts.activeClass);
                    //}
                }
                $(el).isHover(false);
                if (opts.hoverClass)
                    $(el).removeClass(opts.hoverClass);
            }
        },
        clearSelection: function () {
            $(">.d-state-active", this._getItemsContainer()).removeClass("d-state-active");
        },
        getSelectedItems: function () {
            return $(".d-state-active", this._getItemsContainer());
        },
        addItem: function (val) {
            return this._addItem(val);
        },
        _addItem: function (val, _parent) {
            var self = this, opts = this.options, _container = _parent ? _parent : self._getItemsContainer(),
            _item = self._createItemElement();

            if (opts.itemTmpl && $.fn.tmpl) {
                var $tmpl = null;

                if (opts.itemTmpl.jquery)
                    $tmpl = opts.itemTmpl;
                else
                    $tmpl = $(opts.itemTmpl);

                var dataItemEle = $tmpl.tmpl(val);

                if (dataItemEle.length == 1 && dataItemEle[0].tagName == "LI") {
                    if (opts.insertMode == "prepend")
                        dataItemEle.prependTo(_container);
                    else
                        dataItemEle.appendTo(_container);
                    _item = dataItemEle;
                }
                else {
                    if (opts.insertMode == "prepend")
                        _item.prependTo(_container);
                    else
                        _item.appendTo(_container);
                    dataItemEle.appendTo(_item);
                }
            } else {
                if (opts.insertMode == "prepend")
                    _item.prependTo(_container);
                else
                    _item.appendTo(_container);
            }

            if (val) _item.data("dataItem", val);
            self._onItemCreated(_item, val);
            self._triggerEvent("itemCreated", { item: _item, data: val });
            return _item;
        },
        updateItem: function (ele, data) {
            if (this.options.itemTmpl && $.fn.tmpl) {
                var tmplItem = $(ele).tmplItem();
                tmplItem.data = data;
                tmplItem.update();
                var updatedItem = tmplItem.nodes[0];
                $(updatedItem).data("dataItem", data);
                this._onItemCreated(updatedItem, data);
                return updatedItem;
            } else {
                $(ele).data("dataItem", data);
                this._onItemCreated(ele, data);
                return ele;
            }
        },
        _onUpdated: function (data) {
           var item= this.updateItem($(">.d-state-active", this.element), data.result);
           $(item).isActive(true);
        },
        _onRemoved: function (ctx) {
            var item = this.element.children().eq(ctx.key);
            //console.log(item);
            if (item.length)
                item.remove();
        },
        _getItemsContainer: function () {
            return this.element;
        },
        destroy: function () {
            $.dna.taoDataBindingList.prototype.destroy.call(this);
        }
    });
})(jQuery);