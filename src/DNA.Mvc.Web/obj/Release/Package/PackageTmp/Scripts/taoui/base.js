(function ($) {
    $.widget("dna.taoDataBindable", {
        options: {
            dataField: null,
            bindingTo: null,
            mode: "edit" //Possible values : display | edit | new
        },
        _create: function () {
            var opts = this.options;
            this._unobtrusive();
            if (opts.bindingTo)
                this._setBindingSource(opts.bindingTo);
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("mode"))
                opts.mode = el.data("mode");

            if (el.data("bind")) opts.bindingTo = el.datajQuery("bind");

            //Obsolete
            if (el.data("field"))
                opts.dataField = el.data("field");

            if (el.data("bind-field"))
                opts.dataField = el.data("bind-field");

            if (el.attr("name")) 
                opts.dataField = el.attr("name");

            return this;
        },
        _setBindingSource: function (val) {
            var self = this, opts = this.options;
            if (val) {
                //Binding to datasource widget
                if ($.isPlainObject(val)) {
                    throw "Could not bind to none datasource object."
                } else {
                    if (!val.jquery) throw "The input object is not a valid datasource object";
                    var _onPos = function (event, dataItem) {
                        self._onBindingPosition({
                            dataItem: dataItem,
                            value: dataItem[opts.dataField]
                        });
                    };

                    var _prefix = "taoDataSource";

                    if (this._bindSource)
                        this._bindSource.unbind(_prefix + "position", _onPos)
                                                   .unbind(_prefix + "stateChanged", this._onStateChanged);

                    this._bindSource = val;
                    this._bindSource.bind(_prefix + "position", _onPos)
                                               .bind(_prefix + "stateChanged", $.proxy(this._onStateChanged, this));
                }
            }
        },
        getField: function () {
            if (this.options.dataField && this._bindSource)
                return this._bindSource.taoDataSource("field", this.options.dataField);
            return null;
        },
        bindingSource: function () {
            return this._bindSource;
        },
        _onBindingPosition: function (data) { },
        _onStateChanged: function (event, state) { },
        _setOption: function (key, value) {
            if (key == "bindingTo") {
                this._setBindingSource(value);
                return this;
            }

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        }
    });

    $.widget("dna.taoDataBindingList", {
        options: {
            autoBind: true,
            datasource: null,
            itemTmpl: null,
            insertMode: "replace",
            empty: null,
            scroller: null,
            itemCreated: null,
            itemRemoved: null,
            itemsAdded: null
        },
        _setScroller: function (val) {
            var self = this;

            if (val) {
                if (val == "parent")
                    this._scroller = this._getItemsContainer().parent();
                else {
                    if (val == "window" || val == "document" || val == "win" || val == "doc")
                        this._scroller = $(document);
                    else {
                        if (val.jquery)
                            this._scroller = val;
                        else
                            this._scroller = $(val);
                    }
                }
            }

            if (this._scroller) {
                if (this._scroller.jquery && this._scroller.length) {
                    if (this.options.insertMode == "replace")
                        this.options.insertMode = "append";

                    var _handleScrollend = function () {
                        if (!self.element.isDisable()) {
                            if (self._source) {
                                if (self._loading) return;
                                self._loading = true;
                                //console.log("Try scroll end load");
                                if (self._source.length)
                                    self._source.taoDataSource("nextPage").always(function () {
                                        self._loading = false;
                                    });

                            }
                        }
                    };

                    this._scroller.unbind("scroll", _handleScrollend)
                                        .scrollEnd({
                                            children: ".d-items",
                                            callback: _handleScrollend
                                        });
                }
            }

            return this;
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;

            if (el.data("source"))
                opts.datasource = el.datajQuery("source");

            if (el.data("tmpl"))
                opts.itemTmpl = el.datajQuery("tmpl");

            if (el.data("insert-mode")) opts.insertMode = el.data("insert-mode").toLowerCase();
            if (el.data("autobind") != undefined) opts.autoBind = el.dataBool("autobind");

            if (el.data("item-created")) opts.itemCreated = new Function("event", "ui", el.data("item-created"));
            if (el.data("item-removed")) opts.itemRemoved = new Function("event", "ui", el.data("item-removed"));
            if (el.data("items-added")) opts.itemsAdded = new Function("event", "ui", el.data("items-added"));

            if (el.data("scroller")) opts.scroller = el.data("scroller");
            if (el.data("empty")) {
                var emptyEl = el.datajQuery("empty");
                if (emptyEl.length)
                    opts.empty = emptyEl.html();
            }

            if (!opts.datasource) {
                if (el.data("url"))
                    opts.datasource = {
                        actions: {
                            read: el.data("url")
                        }
                    };
            }

            if (el.data("url") && el.data("scroller"))
                opts.datasource.serverPaging = true;

            return this;
        },
        _getItemsContainer: function () { return this.element; },
        _createItemElement: function () {
            ///<summery>
            /// 用于创建数据项元素的方法，当为Grid时则返回的是 tr
            ///</summary>
            return $("<li/>");
        },
        _createDataElements: function (data) {
            var self = this, opts = this.options, _container = self._getItemsContainer(), _items = [];
            if (opts.insertMode == "replace")
                _container.empty();

            if (data) {
                $.each(data, function (i, dat) {
                    _items.push(self._addItem(dat));
                });

                if (_container.children().length == 0)
                    self._setEmpty(true);

                this._triggerEvent("itemsAdded", { items: _items });
            }
            return this;
        },
        _setPosition: function (el) {
            /// <summary>
            /// 同步数据源的数据项位置，并触发数据源的 position 事件
            /// </summary>
            if (!$(el).isDisable()) {
                if ($(el).data("dataItem")) {
                    this._current = { element: el, dataItem: $(el).data("dataItem") };
                    if (this.isBinded())
                        this._source.taoDataSource("pos", $(el).data("dataItem"));
                }
            }
            return this;
        },
        _onItemCreated: function (element, data) {
            /// <summary>
            /// 当数据项元素被创建并添加到容器后执行此方法
            /// </summary>
            ///<remark>可重写此方法对绑定单个数据项元素的事件或附加其它的单项操作处理
            /// 此方法可能由 datasource 的 change, insert 事件触发或由 addItem 方法触发
            ///</remark>
        },
        _onDataChanged: function (results) {
            /// <summary>
            /// 当Widget被绑定到datasource后，datasource 的 changed 被引发后执行此方法
            /// </summary>
            if (this.emptyElement) {
                //this.emptyElement.taoOverlay("destroy");
                this.emptyElement.remove();
            }
            var _items = this._createDataElements(results.data);
            this._getItemsContainer().taoUI();

            if (this._getItemsContainer().children().length == 0)
                this._setEmpty(true);

        },
        _onProcess: function () { },
        _onComplete: function () { },
        _onUpdated: function (val) { ///<summary>通过数据项更新元素内容</summary>  
        },
        _onRemoved: function (val) { ///<summary>通过数据查找并删除元素</summary>
            var self = this;
            if (val) {
                if (val.orginal) {
                    var keyVal = val.orginal[val.key];
                    if (keyVal) {
                        var container = this._getItemsContainer(),
                        _items = $(">.d-item", container);
                        _items.each(function (i, n) {
                            var _itemData = $(n).data("dataItem");
                            if (_itemData) {
                                if (_itemData[val.key]) {
                                    if (_itemData[val.key] == keyVal) {

                                        self._triggerEvent("itemRemoved", { item: $(n), dataItem: _itemData });
                                        $(n).remove();
                                        return;
                                    }
                                }
                            }
                        });
                    }
                }
            }
        },
        _onInserted: function (val) { ///<summary>从DataSource 接收到 inserted 事件的默认处理函数</summary>
            if (val)
                this._addItem(val);
        },
        _onError: function (xhr) { },
        _onCancel: function () { },
        _onPosition: function (dataItem) { },
        _setDataSource: function (datasource) {
            var self = this, opts = this.options;
            if (datasource) {
                if ($.isArray(datasource)) {
                    //If binging the array object as datasourc that means this datasource must be readonly
                    self._createDataElements(datasource);
                }
                else {
                    var _createDataSource = function (val) {
                        var _source = $("<div/>").attr("data-private", true);
                        self.element.after(_source);
                        _source.hide();
                        self._source = _source;
                        _source.taoDataSource(val);
                        self._privateDataSource = true;
                    };

                    //Binding to datasource widget
                    if ($.isPlainObject(datasource)) {
                        _createDataSource(datasource);
                    } else {
                        if ($.type(datasource) == "string") {
                            _createDataSource({
                                actions: {
                                    read: datasource
                                }
                            });
                        } else {
                            if (!datasource.jquery) throw "The input object is not a valid datasource object";
                            this._source = datasource;
                        }
                    }

                    var eventPrefix = "taoDataSource";
                    this._source.bind(eventPrefix + "changed", function (event, results) {
                        self._onDataChanged(results);
                    })
                                            .bind(eventPrefix + "inserted", function (event, results) {
                                                self._onInserted(results);
                                            })
                                            .bind(eventPrefix + "removed", function (event, results) {
                                                self._onRemoved(results);
                                            })
                                            .bind(eventPrefix + "updated", function (event, results) {
                                                self._onUpdated(results);
                                            })
                                            .bind(eventPrefix + "error", function (event, xrh) {
                                                self._onError(xrh);
                                            })
                                            .bind(eventPrefix + "process", function (event) {
                                                self._onProcess();
                                            })
                                            .bind(eventPrefix + "complete", function (event) {
                                                self._onComplete();
                                            })
                                            .bind(eventPrefix + "cancel", function (event) {
                                                self._onCancel();
                                            })
                                            .bind(eventPrefix + "position", function (event, dataItem) {
                                                if (self.currentData() != dataItem) {
                                                    self._current = dataItem;
                                                    self._onPosition(dataItem);
                                                }
                                            });

                    if (opts.autoBind)
                        self.databind();
                }
            }
            return this;
        },
        _setOption: function (key, value) {
            if (key == "datasource") {
                this.options.datasource = value;
                this._setDataSource(value);
                return this;
            }

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _addItem: function (val, _parent) {
            ///<summary>通过数据项向元素容器增加新元素</summary>
            var self = this, opts = this.options, _container = _parent ? _parent : self._getItemsContainer(),
            _item = self._createItemElement();
            if (opts.insertMode == "prepend")
                _item.prependTo(_container);
            else
                _item.appendTo(_container);

            if (opts.itemTmpl && $.fn.tmpl) {
                var $tmpl = null;

                if (opts.itemTmpl.jquery)
                    $tmpl = opts.itemTmpl;
                else
                    $tmpl = $(opts.itemTmpl);

                $tmpl.tmpl(val)
                         .appendTo(_item);
            }

            if (val) _item.data("dataItem", val);
            self._onItemCreated(_item, val);
            self._triggerEvent("itemCreated", { item: _item, data: val });
            return _item;
        },
        isBinded: function () {
            return (this._source != undefined) && (this._source != null);
        },
        currentData: function () {
            ///<summary>Returns current data item</summary>
            if (this._current != undefined)
                return this._current;
            return null;
        },
        databind: function (callback) {
            /// <summary>
            /// 马上调用Datasource 的read方法从数据源获取数据
            /// </summary>
            if (this._source && this._source.length) {
                this._source.taoDataSource("read")
                       .done(function (results) {
                           if ($.isFunction(callback))
                               callback(results);
                       });
            }
            return this;
        },
        clear: function () {
            ///<summary>Clear all items in the list</summary>
            this._getItemsContainer().empty();
        },
        viewItems: function () {
            return $(".d-item", this._getItemsContainer());
        },
        viewData: function () {
            var _items = this.viewItems(), _dataItems = [];
            _items.each(function (i, item) {
                _dataItems.push(item.data("dataItem"));
            });
            return _dataItems;
        },
        at: function (index) {
            var items = this.viewItems();
            if (items.length > index)
                return $(items.get(index));
            return null;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        _setEmpty: function (val) {
            var opts = this.options;
            if (this.emptyElement)
                //this.emptyElement.taoOverlay("destroy");
                this.emptyElement.remove();


            if (val) {
                if (opts.empty) {
                    var emptyHtml = $.isFunction(opts.empty) ? opts.empty() : opts.empty;
                    this.emptyElement = $(emptyHtml).appendTo(this.element);
                    //this.emptyElement = $(emptyHtml).appendTo("body");
                    //this.emptyElement.show().taoOverlay({ target: this._getItemsContainer(), autoOpen: true });
                }
            }

        },
        datasource: function () {
            return this._source;
        },
        destroy: function () {
            this._setEmpty(false);
            if (this._privateDataSource && this._source)
                this._source.remove();
            $.Widget.prototype.destroy.call(this);
        }
    });

    $.widget("dna.taoHierarchical", $.dna.taoDataBindingList, {
        options: {
            preload: false
        },
        _read: function (url) {
            //read data from data source helper method
            var self = this;
            if (this._source) {
                if (url != undefined)
                    return this._source.taoDataSource("read", url);
                else
                    return this._source.taoDataSource("read");
            }
            return $.Deferred().resolve({ data: [] });
        },
        _popupAttrs: function (key, val, item) {
            ///<summary>Use to popuplate the additional attributes and add to element item. </summary>
            return false;
        },
        _onItemCreated: function (item, dat) { /*Need override*/
            item.data("dataItem", dat);
            var link = null;

            if (!dat.html && (this.options.itemTmpl==undefined || this.options.itemTmpl==null))
                link = $("<a/>").addClass("d-inline").attr("href","javascript:void(0);").appendTo(item);

            if ($.isPlainObject(dat)) {
                for (var pro in dat) {
                    if ($.isArray(dat[pro]) || $.isPlainObject(dat[pro]))
                        continue;

                    if (!dat.html) {
                        if (pro == "text") {
                            if (dat.text)
                                link.text(dat.text);
                            continue;
                        }

                        if (pro == "link") {
                            if (dat.link)
                                link.attr("href", dat.link);
                            else
                                link.attr("href", "javascript:void(0);");
                            continue;
                        }

                        if (pro == "target")
                            link.attr("target", dat.target);

                        if (pro == "img") {
                            if (dat.img)
                                $("<img/>").attr("src", dat.img).addClass("d-inline").prependTo(link);
                            continue;
                        }

                        if (pro == "icon") {
                            if (dat.icon) {
                                $("<span/>").addClass("d-item-icon").addClass(dat.icon).prependTo(link);
                            }
                            continue;
                        }

                        if (pro == "url") {
                            if (dat.url) {
                                item.data("url", dat.url);
                                if (this.options.preload)
                                    $.preload(dat.url);
                            }
                            continue;
                        }
                    }

                    if (pro == "title") {
                        if (dat.title)
                            item.attr("title", dat.title);
                        continue;
                    }

                    if (pro == "disabled") {
                        if (dat.disabled != undefined)
                            item.isDisable(dat.disabled);
                        continue;
                    }

                    if (pro == "selected") {
                        if (dat.selected != undefined)
                            item.isActive(dat.selected);
                        continue;
                    }

                    if (this._popupAttrs(pro, dat[pro], item))
                        continue;

                    if (pro == "html") {
                        var _contentHtml = "";

                        if ($.isFunction(dat.html)) {
                            var _contentHanlder = $.proxy(dat.html, item);
                            _contentHtml = _contentHanlder();
                        } else
                            _contentHtml = dat.html;

                        var _contentElement = $(_contentHtml);
                        item.empty().append(_contentElement).addClass("d-item custom");
                        item.taoUI();
                        continue;
                    }

                    if (pro == "click") {
                        if ($.isFunction(dat[pro]))
                            item.bind("click", dat[pro]);
                        continue;
                    }

                    item.attr("data-" + pro, dat[pro]);
                }
            } else {
                if (link)
                    link.text(dat);
                else
                    item.text(dat);
            }


            return this;
        },
        _createDataElements: function (data, parent) {
            var self = this, opts = this.options, container = parent ? parent : this.element,
            bindRecursive = function (_data, _parent) {
                for (var i = 0; i < _data.length; i++) {
                    var dat = _data[i];
                    var _item = self._addItem(dat, _parent);
                    if (dat.children) {
                        if ($.isArray(dat.children)) {
                            if (dat.children.length > 0) {
                                var thisContainer = $("<ul/>").appendTo(_item);
                                bindRecursive(dat.children, thisContainer);
                            }
                        }
                    }
                }
            }

            bindRecursive(data, container);
            self._initItems(container);
            return this;
        },
        _onDataChanged: function (results) {
            return this;
        },
        _initItems: function (itemContainer) {
            var _container = itemContainer ? itemContainer : this.element, self = this;
            $("li", _container).each(function (i, n) {
                self._initItem(n);
            });
            return this;
        },
        _initItem: function (item) { /*Need override*/ },
        _setPreload: function (val) {
            if (val)
                this.element.find("a[href]").preload();
            return this;
        }
    });

})(jQuery);