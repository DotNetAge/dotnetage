/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoTextbox", $.dna.taoDataBindable, {
        options:
        {
            value: "",
            waterMark: null,
            width: 0,
            height: 0,
            iconClass: null,
            iconImg: null,
            readonly: false,
            disabled: false,
            iconClick: null,
            autoComplete: null,
            speech: false, // Only avaliable for chrome
            fontSize: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;

            this._unobtrusive();

            if (el.val()) opts.value = el.val();
            if (opts.width) el.width(opts.width);
            if (opts.height) el.height(opts.height);

            if (opts.iconClick)
                el.bind(eventPrefix + "iconclick", opts.loaded);


            el.wrap("<div/>");
            var $parent = el.parent();
            $parent.addClass("d-reset d-ui-widget d-input d-textbox");

            if (opts.readonly || el.attr("readonly"))
                self._setreadonly(true);

            if (el.attr("style")) {
                $parent.attr("style", el.attr("style"));
                    //.css("width", "auto");
            }

            if (el.attr("class")) {
                $parent.addClass(el.attr("class"));
                el.removeAttr("class");
            }

            el.addClass("d-ui-widget-content");

            this.container = $parent;

            if (opts.iconClass)
                this._seticon(opts.iconClass);
            else
                this._setimg(opts.iconImg);

            //if ($(document))
            //  this._createwatermark(opts.waterMark);

            var _setWaterMark = function () {
                if (el.val()) {
                    var wm = $(">.d-watermark", el.parent());
                    if (wm.length > 0)
                        wm.hide();
                }
            };

            el.hover(function () {
                if (!$parent.isActive())
                    $parent.isHover(true);
            },
             function () {
                 $parent.isHover(false);
             });

            el.bind("focus", function () {
                if (!$parent.isDisable() && !$parent.isReadonly()) {
                    $parent.isHover(false);
                    $parent.isActive(true);
                }
            })
               .bind("blur", function () {
                   if (!$parent.isReadonly())
                       $parent.isActive(false);

                   //if ($.browser.msie)
                   //  _setWaterMark();
               });

            //self._setsize();

            if (opts.bindingTo)
                this._setBindingSource(opts.bindingTo);

            //if ($.browser.msie) 
            //    _setWaterMark();

            if (opts.autoComplete)
                el.taoAutoComplete(opts.autoComplete);

            if (opts.speech)
                el.attr("x-webkit-speech", "");

            if (el.attr("disabled") == "disabled")
                el.parent().isDisable(true);

            //if (opts.button)
            //    el.parent().append(opts.button);
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("watermark")) opts.waterMark = el.data("watermark");
            if (el.data("icon")) opts.iconClass = el.data("icon");
            if (el.data("img")) opts.iconImg = el.data("img");
            if (el.data("height")) opts.height = el.dataInt("height");
            if (el.data("width")) opts.width = el.dataInt("width");
            if (el.data("icon-click")) opts.iconClick = new Function("event", "ui", el.data("icon-click"));
            if (el.attr("disabled"))
                opts.disabled = (el.attr("disabled") == "disabled" || el.attr("disabled") == true) ? true : false;
            if (el.attr("readonly"))
                opts.readonly = (el.attr("readonly") == "readonly" || el.attr("readonly") == true) ? true : false;
            if (el.data("button"))
                opts.button = el.datajQuery("button");
            if (el.data("autocomplete-source")) {
                opts.autoComplete = {
                    datasource: el.datajQuery("autocomplete-source")
                };
            }
            if (el.data("speech") != undefined) opts.speech = el.dataBool("speech");
            if (el.attr("placeholder")) opts.waterMark = el.attr("placeholder");
            if (el.data("autocomplete")) {
                opts.autoComplete = {
                    datasource: el.data("autocomplete"),
                    param: el.data("autocomplete-param") ? el.data("autocomplete-param") : "q",
                    valueField: el.data("autocomplete-val-field") ? el.data("autocomplete-val-field") : "text"
                };
            }

            return $.dna.taoDataBindable.prototype._unobtrusive.call(this);
            //return this;
        },
        _setreadonly: function (value) {
            this.element.parent().isReadonly(value);
            this.element.attr("readonly", value);
            this.options.readonly = value;
        },
        _seticon: function (icon) {
            var el = this.element, self = this;
            var _ico = $(">.d-textbox-icon", this.element.parent());
            if (icon) {
                if (_ico.length == 0)
                    _ico = $("<span/>").addClass(icon)
                                              .addClass("d-textbox-icon")
                                              .prependTo(el.parent())
                                              .click(function () {
                                                  self._triggerEvent("iconclick");
                                                  el.focus();
                                              });
                else
                    _ico.attr("class", "d-textbox-icon " + icon);
            }
            else {
                if (_ico.length) {
                    _ico.remove();
                }
            }
            this._setwatermarkpadding();
        },
        _setimg: function (img) {
            var el = this.element, _img = $(">.d-textbox-icon", el.parent());
            if (img) {
                if (_img.length == 0) {
                    _img = $("<span/>").addClass("d-textbox-icon")
                                                          .css({ "background-image": "url(" + img + ")" })
                                                          .prependTo(el.parent())
                                                          .click(function () {
                                                              self._triggerEvent("iconclick");
                                                              el.focus();
                                                          });
                }
                else
                    _img.css("background-image", "url(" + img + ")");
            }
            else {
                if (_img.length) {
                    _img.remove();
                }
            }
            this._setwatermarkpadding();

        },
        _setsize: function () {
            //var _icon = $(">.d-textbox-icon", this.element.parent()),
            //_w = this.element.parent().width();
            //if (_icon.length)
            //    this.element.width(_w - _icon.outerWidth(true));
            //else
            //    this.element.width(_w);
            return this;
        },
        _setwatermarkpadding: function () {
            var opts = this.options, el = this.element;
            var wm = wm = $(">.d-watermark", el.parent());
            if (wm.length) {
                if (opts.iconImg || opts.iconClass)
                    wm.css({ "padding-left": $(">.d-textbox-icon", el.parent()).outerWidth(true) + "px" });
                else {
                    wm.removeAttr("style");
                }
                wm.height(el.parent().height())
                     .width(el.parent().width());
            }
        },
        _createwatermark: function (val) {
            var opts = this.options, el = this.element, wm = $(">.d-watermark", el.parent());

            if (wm.length == 0) {
                if (val) {
                    wm = $("<div/>").addClass("d-watermark")
                                           .appendTo(el.parent())
                                           .text(val)
                                           .click(function () { el.focus() });

                    this._setwatermarkpadding();
                }
            }
            else {
                wm.text(val);
            }
        },
        disable: function () {
            this.element.parent().isDisable(true);
            this.element.attr("disabled", true);
        },
        enable: function () {
            this.element.parent().isDisable(false);
            this.element.attr("disabled", false);
        },
        _setOption: function (key, value) {
            if (key == "value") {
                this.element.val(value);
                return this;
            }

            if (key == "width") {
                this.element.width(value);
                this._setsize();
                return this;
            }

            if (key == "height") {
                this.element.height(value);
                return this;
            }

            if (key == "waterMark") {
                this._createwatermark(value);
                return this;
            }

            if (key == "iconClass") {
                this._seticon(value);
                this._setsize();
                return this;
            }

            if (key == "iconImg") {
                this._setimg(value);
                this._setsize();
                return this;
            }

            if (key == "readonly") {
                this._setreadonly(value);
                return this;
            }

            if (key == "disabled") {
                if (value) this.disable();
                else this.enable();
            }

            return $.dna.taoDataBindable.prototype._setOption.call(this, key, value);
        },
        _onBindingPosition: function (data) {
            var opts = this.options;

            if (opts.mode != "new") {
                if (data.value != undefined)
                    this.element.val(data.value);
            }
            return this;
        },
        _onStateChanged: function (event, state) {
            if (state == "inserted" || state == "removed" || state == "add")
                this.element.val("");
            return this;
        },
        _setBindingSource: function (val) {
            $.dna.taoDataBindable.prototype._setBindingSource.call(this, val);
            var field = this.getField();
            if (field) {
                if (field.desc)
                    this.element.attr(field.desc);
                else {
                    if (field.title)
                        this.element.attr(field.title);
                }
            }
            return this;
        },
        destroy: function () {
            var wrapper = this.element.parent();
            wrapper.after(this.element);
            wrapper.remove();
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);