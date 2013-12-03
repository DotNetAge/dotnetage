/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoAutoComplete", {
        options: {
            datasource: null, //The possible value is 
            param: "q",
            valueField: "text",
            width: 200,
            complete:null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive(el);

            if (opts.complete)
                el.bind(eventPrefix + "complete", opts.complete);

            if (el[0].tagName.toLowerCase() == "input") {

                if (opts.datasource)
                    this._setDataSource(opts.datasource);

                el.keyup(function (event) {
                    var ignores = [45, 35, 36, 33, 34, 17, 16, 18, 13, 20, 32, 91, 93, 188, 27];
                    if ($.inArray(event.which, ignores) > -1) {
                        //console.log(event.which);
                        return;
                    }

                    var searchTerm = el.val();
                    if (searchTerm) {
                        if (self._source) {
                            if (self._source.taoDataSource("option", "serverFiltering"))
                                searchTerm.filter({ field: opts.param, operator: "contains", val: searchTerm }, searchTerm);
                            else
                                self._source.taoDataSource("read");
                        }
                    }
                });
            } else {
                console.log("AutoComplete widget must be apply to input element.");
            }
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;
            $.dna.taoListview.prototype._unobtrusive.call(this, element);
            if (el.data("valuefield")) opts.valueField = el.data("valuefield");

            if (el.data("autocomplete-label"))
                opts.valueField = el.data("autocomplete-label");

            if (el.data("autocomplete-param")) opts.param = el.data("autocomplete-param");
            if (el.data("autocomplete")) opts.datasource = el.data("autocomplete");
            if (el.data("autocomplete-width")) opts.width = el.dataInt("autocomplete-width");
            if (el.data("autocomplete-select")) opts.complete = new Function("event", "ui", el.data("autocomplete-select"));

            if (el.data("autocomplete-map"))
                this.map = new Function("data", el.data("autocomplete-map"));

            if (el.data("autocomplete-convert"))
                this.convert = new Function("data", el.data("autocomplete-convert"));

            return this;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
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
                            var _searchParam = {};
                            _searchParam[opts.param] = function () {
                                return self.element.val();
                            };

                            _createDataSource({
                                actions: {
                                    read: {
                                        url: datasource,
                                        data: _searchParam
                                    }
                                }
                            });
                        } else {
                            if (!datasource.jquery) throw "The input object is not a valid datasource object";
                            this._source = datasource;
                        }
                    }

                    var eventPrefix = "taoDataSource";
                    this._source.bind(eventPrefix + "changed", $.proxy(this._onResult, this));
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
        _onResult: function (event, results) {
            var self = this, opts = this.options;
            if (this._menu) {
                this._menu.remove();
                this._menu = null;
            }

            if (results) {
                if (results.data) {
                    var el = this.element;
                    var _of = el;
                    if (_of.is("[data-role='textbox']"))
                        _of = el.parent();

                    var formattedData = results.data;

                    if ($.isFunction(this.map)) {
                        formattedData = this.map(results.data);
                    }

                    if ($.isFunction(this.convert)) {
                        //formattedData = this.map(results.data);
                        var cached = [];
                        $.each(formattedData, function (i, di) {
                            var cr = self.convert(di);
                            cached.push(cr);
                        });
                        formattedData = cached;
                    }

                    this._menu = $("<ul/>").css({ "position": "absolute", "top": "0px", "left": "0px", "z-index":$.topMostIndex() , "width": el.width() + "px" })
                                                      .appendTo("body")
                                                      .taoMenu({
                                                          type: "vertical",
                                                          datasource: formattedData,
                                                          itemClick: function (evt, ui) {
                                                              var _item = $(ui.item);
                                                              if (_item.data("dataItem")) {
                                                                  var _dataItem = _item.data("dataItem");
                                                                  if ($.isPlainObject(_dataItem)) {
                                                                      if (opts.valueField)
                                                                          el.val(_dataItem[opts.valueField]);
                                                                  }
                                                                  else {
                                                                      el.val(_dataItem);
                                                                  }
                                                              } else {
                                                                  el.val(_item.text());
                                                              }
                                                              self._menu.hide().remove();
                                                              self._triggerEvent("complete", _dataItem);
                                                          }
                                                      })
                                                       .position({
                                                           of: _of,
                                                           at: "left bottom",
                                                           my: "left top"
                                                       });

                    $(document).one("click",function () {
                        if (self._menu) {
                            self._menu.remove();
                            self._menu = null;
                        }
                    });
                }
            }
        },
        destroy: function () {
            if (this._menu)
                this._menu.remove();
            if (this._source)
                this._source.remove();
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);