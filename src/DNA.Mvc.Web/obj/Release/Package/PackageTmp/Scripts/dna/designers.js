(function ($) {

    $.widget("dna.editorbase", {
        options: {
            target: null,
            change: null,
            //  beforeApply: null,
            stylesheet: null,
            win: null
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options, self = this, doc = null;
            if (el.attr("data-win")) opts.win = el.data("win");
            if (opts.win)
                doc = $("#" + opts.win).length ? $("#" + opts.win)[0].contentWindow.document : null;

            if (el.attr("data-target")) opts.target = el.datajQuery("target", doc);
            if (el.attr("data-stylesheet")) opts.stylesheet = el.datajQuery("stylesheet", doc);
            if (el.attr("data-change")) opts.change = new Function("event", "ui", el.attr("data-change"));

            if (opts.stylesheet) {
                if (opts.stylesheet.length == 0) {
                    $("<style id=\"" + el.data("stylesheet") + "\"/>").appendTo($("head", document));
                    opts.stylesheet = $("#" + el.data("stylesheet"));
                }

                el.bind("stylechange", function (event, ui) {
                    var lines = [], csstext = "", _target = ui.target;
                    // el.trigger("beforeApply");

                    //1.st find which editor response for this style
                    $(".d-style-editor[data-target='" + _target + "'][data-css],.d-style-editor[data-target-selector='" + _target + "'][data-css]").each(function (i, n) {
                        var _l = $(n).attr("data-css"), _concat = $(n).attr("data-concat");
                        if (_concat)
                            _l += ";" + _concat;
                        lines.push(_l);
                    });

                    //2. combine the style inline
                    csstext = lines.join(";");

                    if (csstext && !csstext.endsWith(";"))
                        csstext += ";";

                    //3.replace the new style line to style element
                    var editing_text = opts.stylesheet.text();

                    //How to match whitespace ???
                    //var startReg=new RegExp("*","g");
                    var formattedTargetStr = _target.replace(/\*/g, "\\*");
                    formattedTargetStr = formattedTargetStr.replace(/\(/g, "\\(");
                    formattedTargetStr = formattedTargetStr.replace(/\)/g, "\\)");
                    formattedTargetStr = formattedTargetStr.replace(/\[/g, "\\[");
                    formattedTargetStr = formattedTargetStr.replace(/\]/g, "\\]");

                    var regex = new RegExp(formattedTargetStr + " \{(.+?)\}");
                    if (editing_text.match(regex))
                        editing_text = editing_text.replace(regex, _target + " {" + csstext + "}");
                    else
                        editing_text += _target + " {" + csstext + "}\n";

                    opts.stylesheet.text(editing_text);
                    //opts.stylesheet.trigger("change");
                });
            }

            if (opts.change)
                el.bind("stylechange", opts.change);
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            var _t = opts.target[0];

            //Remarks: The DOMAttrModified Event is low preformance way. The MutationObserver will be good but only support FF and Chrome
            //https://developer.mozilla.org/zh-CN/docs/DOM/MutationObserver
            el.addClass("d-style-editor");
            this._doChange = function (e) {
                var prop = e.propertyName ? e.propertyName : e.attrName;
                if (e.target == _t || e.srcElement == _t) {
                    if (prop == "class" || prop == "style") {
                        self._onStyleChange();
                    }
                }
            }
            this._bindChange = function () {
                if (opts.target) {
                    var elemToCheck = opts.target[0];
                    if (elemToCheck) {
                        if (elemToCheck.addEventListener) { // all browsers except IE before version 9
                            elemToCheck.addEventListener('DOMAttrModified', self._doChange, false);    // Firefox, Opera, IE
                        }

                        if (elemToCheck.attachEvent) {  // Internet Explorer and Opera
                            elemToCheck.attachEvent('onpropertychange', self._doChange);   // Internet Explorer
                        }
                    }
                }
            }
            this._unbindChange = function () {
                if (opts.target) {
                    //opts.target.unbind("onpropertychange", this._doChange)
                    //                  .unbind("DOMAttrModified", this._doChange);
                    var elemToCheck = opts.target[0];
                    if (elemToCheck) {
                        if (elemToCheck.removeEventListener) { // all browsers except IE before version 9
                            elemToCheck.removeEventListener('DOMAttrModified', self._doChange, false);    // Firefox, Opera, IE
                        }

                        if (elemToCheck.detachEvent) {  // Internet Explorer and Opera
                            elemToCheck.detachEvent('onpropertychange', self._doChange);   // Internet Explorer
                        }
                    }
                }
            }
            this._initUI();
            //read style from target element
            // this._onStyleChange();
            //  this._bindChange();
            el[0].refresh = function () {
                el.removeAttr("data-css");
                el.empty();
                self._initUI();
                var cssText = self.csstext();
                if (cssText)
                    el.attr("data-css", cssText);
                //self._onStyleChange();
            };
        },
        setStyle: function (styleObj) {
            var _target = this.options.target;
            if (_target || this.options.stylesheet) {
                // this._unbindChange();
                //  if (this.styleholder)
                //     this.styleholder.css(styleObj);
                // else
                if (!this.options.stylesheet)
                    $(_target).css(styleObj);
                //this._bindChange();
            }

            return this.element.attr("data-css", this.csstext())
                          .trigger("stylechange", { target: _target ? _target.selector : null });
        },
        _initUI: function () { },
        _onStyleChange: function () { },
        csstext: function () { },
        destroy: function () {
            this._unbindChange();
            $.Widget.prototype.destroy.call(this);
        }
    });

    $.widget("dna.corners_editor", $.dna.editorbase, {
        options: { lockText: "Lock corners together" },
        _initUI: function () {
            var self = this, opts = this.options, el = this.element;
            //$.dna.editorbase.prototype._create.call(this);
            var topWrapper = $("<div/>").appendTo(el).css("margin-bottom", "5px"),
                bottomWrapper = $("<div/>").appendTo(el),
                lockWrapper = $("<div/>").appendTo(el),
                    _setCorner = function (input, holder) {
                        var cssObj = {};
                        if (locker.attr("checked")) {
                            cssObj = {
                                "border-radius": input.val() + "px",
                                "-moz-border-radius": input.val() + "px",
                                "-webkit-border-radius": input.val() + "px"
                            };
                            $("input[type=number]", el).not(input).val(input.val());
                            $(".corner-holder", el).css(cssObj);
                        } else {
                            cssObj[input.attr("name")] = input.val() + "px";
                            //cssObj["-webkit-" + input.attr("name")] = input.val() + "px";
                            //cssObj["-moz-" + input.attr("name")] = input.val() + "px";
                        }

                        holder.css(cssObj);
                        self.setStyle(cssObj);
                        //el.attr("data-css", self.csstext());
                    },
                _getRadiusVal = function (prop) {
                    v = el.css(prop);
                    if (v)
                        return v.replace("px", "");
                    else
                        return 0;
                };

            //Top corner inputs
            var tlr_input = $("<input type='number' name='border-top-left-radius'/>").attr("data-role", "none")
                                                                                                                                   .appendTo(topWrapper)
                                                                                                                                   .val(_getRadiusVal("border-top-left-radius"))
                                                                                                                                   .bind("change", function () { _setCorner($(this), tlr_holder); }),

             tlr_holder = $("<div/>").appendTo(topWrapper)
                                                  .addClass("corner-holder")
                                                  .attr("data-hold", "border-top-left-radius")
                                                  .css({
                                                      margin: "0px 10px",
                                                      "border-left": "5px solid #ccc",
                                                      "border-top": "5px solid #ccc",
                                                      "border-top-left-radius": "5px"
                                                  }).width(30).height(20),
           trr_holder = $("<div/>").appendTo(topWrapper).addClass("corner-holder")
                .attr("data-hold", "border-top-right-radius")
                             .css({
                                 margin: "0px 10px",
                                 "border-right": "5px solid #ccc",
                                 "border-top": "5px solid #ccc",
                                 "border-top-right-radius": "5px"
                             }).width(30).height(20),

            trr_input = $("<input type='number'  name='border-top-right-radius'/>").attr("data-role", "none").val(_getRadiusVal("border-top-right-radius"))
                .appendTo(topWrapper)
            .bind("change", function () { _setCorner($(this), trr_holder); }),

            blr_input = $("<input type='number' name='border-bottom-left-radius'/>").attr("data-role", "none").val(_getRadiusVal("border-bottom-left-radius"))
                .appendTo(bottomWrapper)
            .bind("change", function () { _setCorner($(this), blr_holder); }),

            blr_holder = $("<div/>").appendTo(bottomWrapper).addClass("corner-holder")
                .attr("data-hold", "border-bottom-left-radius")
                             .css({
                                 margin: "0px 10px",
                                 "border-left": "5px solid #ccc",
                                 "border-bottom": "5px solid #ccc",
                                 "border-bottom-left-radius": "5px"
                             }).width(30).height(20),
           brr_holder = $("<div/>").appendTo(bottomWrapper).addClass("corner-holder")
                .attr("data-hold", "border-bottom-right-radius")
                .css({
                    margin: "0px 10px",
                    "border-right": "5px solid #ccc",
                    "border-bottom": "5px solid #ccc",
                    "border-bottom-right-radius": "5px"
                }).width(30).height(20),
            brr_input = $("<input type='number'  name='border-bottom-right-radius'/>").attr("data-role", "none").val(_getRadiusVal("border-bottom-right-radius"))
                .bind("change", function () { _setCorner($(this), brr_holder); })
                .appendTo(bottomWrapper),

               locker = $("<input type='checkbox' checked='checked'/>").appendTo(lockWrapper).attr("data-label", opts.lockText).attr("data-role", "none");
            locker.taoCheckbox();
            trr_input.after($("<span/>").text("px"));
            tlr_input.after($("<span/>").text("px"));
            brr_input.after($("<span/>").text("px"));
            blr_input.after($("<span/>").text("px"));

            topWrapper.children().addClass("d-inline");
            bottomWrapper.children().addClass("d-inline");
            $("input[type=number]").width(40).bind("focus", function () { $(this).select(); });
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            el.addClass("d-corners-editor");
            $.dna.editorbase.prototype._unobtrusive.call(this);
            if (el.attr("data-text")) opts.lockText = el.datajQuery("text");
        },
        _onStyleChange: function () {
            var _target = this.options.target, el = this.element, inputs = $("input[type=number]", el);
            if (_target) {
                inputs.each(function (i, n) {
                    var prop = $(n).attr("name"),
                        v = _target.css(prop);
                    if (v)
                        $(n).val(v.replace("px", ""));
                    else
                        $(n).val(0);

                    var m = {};

                    m[prop] = v;
                    m["-webkit-" + prop] = v;
                    m["-moz-" + prop] = v;

                    $("[data-hold='" + prop + "']", el).css(m);
                });
            }
        },
        csstext: function () {
            var _target = this.options.target, el = this.element, inputs = $("input[type=number]", el)
            props = [],
            isLock = $("input[type=checkbox]", el).attr("checked");

            if (isLock == "checked") {
                var n = $("input[type=number]", el).first(), v = $(n).val() + "px";
                props.push("border-radius:" + v);
                props.push("-webkit-border-radius:" + v);
                props.push("-moz-border-radius:" + v);
            } else {
                inputs.each(function (i, n) {
                    var prop = $(n).attr("name"),
                        v = $(n).val() + "px";
                    props.push(prop + ":" + v);
                    //    props.push("-webkit-" + prop + ":" + v);
                    //  props.push("-moz-" + prop + ":" + v);
                });
            }
            return props.join(";");

        },
        destroy: function () {
            this.element.empty();
            $.dna.editorbase.prototype.destroy.call(this);
        }
    });

    $.widget("dna.colors_editor", $.dna.editorbase, {
        options: {
            border: true,
            background: true,
            text: true,
            borderText: "Border",
            colorText: "Text",
            bgText: "Background"
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            el.addClass("d-colors-editor");
            $.dna.editorbase.prototype._unobtrusive.call(this);
            if (el.attr("data-border") != undefined) opts.border = el.dataBool("border");
            if (el.attr("data-background") != undefined) opts.background = el.dataBool("background");
            if (el.attr("data-text") != undefined) opts.text = el.dataBool("text");
            if (el.attr("data-text-border")) opts.borderText = el.data("text-border");
            if (el.attr("data-text-background")) opts.bgText = el.data("text-background");
            if (el.attr("data-text-color")) opts.colorText = el.data("text-color");
        },
        _initUI: function () {
            var self = this, opts = this.options, el = this.element,
                _createEditor = function (name, label) {
                    var _color = opts.target ? opts.target.css(name) : "";
                    if (opts.target && name == "border-color" && _color == "")
                        _color = opts.target.css("border-top-color");

                    var container = $("<div/>").appendTo(el),
                        lb = $("<label/>").appendTo(container)
                                                    .text(label)
                                                    .addClass("d-inline"),
                        input = $("<input name='" + name + "'/>").attr("data-role", "none")
                        .width(80)
                        .appendTo(container)
                        .val(_color)
                        .taoColorDropdown({
                            palette: "fa",
                            //color:_color,
                            change: function (event, color) {
                                var o = {};
                                o[input.attr("name")] = color;
                                self.setStyle(o);
                            }
                        });
                    $("input.d-content-text", container).focus(function () { $(this).select(); });
                };

            if (opts.text)
                _createEditor("color", opts.colorText);

            if (opts.background)
                _createEditor("background-color", opts.bgText);

            if (opts.border)
                _createEditor("border-color", opts.borderText);
        },
        _onStyleChange: function () {
            var _target = this.options.target,
                el = this.element,
                inputs = $("input.d-content-val", el);

            if (_target) {
                inputs.each(function (i, colorInput) {
                    var prop = $(colorInput).attr("name"),
                        _color = _target.css(prop);
                    //if ($(n).hasClass("d-content-val"))
                    $(colorInput).taoColorDropdown("option", "color", _color);
                    //else
                    //    $(n).val(_color);
                });
            }
            return el;
        },
        csstext: function () {
            var el = this.element, inputs = $("input[name]", el), props = [];
            inputs.each(function (i, n) {
                var _name = $(n).attr("name"),
                    _v = $(n).val();
                if (_v)
                    props.push(_name + ":" + (_v ? _v : "transparent"));

                //if (_v && _name == "background-color")
                //props.push("background-image:none");

            });
            return props.join(";");
        },
        destroy: function () {
            $.dna.editorbase.prototype.destroy.call(this);
        }
    });

    $.widget("dna.background_editor", $.dna.editorbase, {
        options: {
            scalingText: "Image scaling",
            posText: "Position",
            fullText: "Full",
            fitText: "Fit",
            titleText: "Title",
            titleHText: "Title horizontally",
            titleVText: "Title vertically",
            normalText: "Normal",
            addImgText: "Add image",
            removeText: "Remove image",
            emptyImg: "/content/images/add_image_thumb.png",
            netdrive: false
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            $.dna.editorbase.prototype._unobtrusive.call(this);
            el.addClass("d-background-editor");
            if (el.attr("data-text-scaling")) opts.scalingText = el.data("text-scaling");
            if (el.attr("data-text-pos")) opts.posText = el.data("text-pos");
            if (el.attr("data-text-full")) opts.fullText = el.data("text-full");
            if (el.attr("data-text-fit")) opts.fitText = el.data("text-fit");
            if (el.attr("data-text-title")) opts.titleText = el.data("text-title");
            if (el.attr("data-text-h")) opts.titleHText = el.data("text-h");
            if (el.attr("data-text-v")) opts.titleVText = el.data("text-v");
            if (el.attr("data-text-normal")) opts.normalText = el.data("text-normal");
            if (el.attr("data-text-addimg")) opts.addImgText = el.data("text-addimg");
            if (el.attr("data-text-remove")) opts.removeText = el.data("text-remove");
            if (el.attr("data-netdrive") != undefined) opts.netdrive = el.data("netdrive");
        },
        _initUI: function () {
            var self = this, el = this.element, opts = this.options,
                imgContainer = $("<div/>").appendTo(el).css("margin-bottom", "10px"),
                thumbContainer = $("<div/>").addClass("d-inline")
                .appendTo(imgContainer),

                imgButtons = $("<div/>").addClass("d-inline")
                .appendTo(imgContainer)
                .css({
                    "padding": "0px 10px",
                    "margin-left": "15px"
                }),

                scalingContainer = $("<div/>").appendTo(el)
                .addClass("d-inline")
                .css("font-size", "9pt")
                .append($("<div/>").text(opts.scalingText)),

            thumb = $("<div/>").css({
                "background-image": "url(" + opts.emptyImg + ")",
                "background-size": "65px",
                "background-repeat": "no-repeat",
                "background-position": "center center"
            })
                                              .height(65)
                                              .width(65)
                                              .appendTo(thumbContainer),

            posContainer = $("<div/>").appendTo(el)
                .addClass("d-inline")
                .append($("<div/>")
                .text(opts.posText))
                .css({
                    "vertical-align": "top",
                    "font-size": "9pt"
                });

            var radios = $("<div/>").data("data-value", "no-repeat").appendTo(scalingContainer);

            if (opts.target)
                radios.data("data-value", opts.target.css("background-repeat"))

            $("<input type='radio' />").attr("data-label", opts.fullText).val("cover").appendTo(radios);

            $("<input type='radio' />").attr("data-label", opts.fitText).val("contain").appendTo(radios);

            $("<input type='radio' />").attr("data-label", opts.titleText).val("repeat").appendTo(radios);

            $("<input type='radio' />").attr("data-label", opts.titleHText).val("repeat no-repeat").appendTo(radios);

            $("<input type='radio' />").attr("data-label", opts.titleVText).val("no-repeat repeat").appendTo(radios);

            $("<input type='radio' />").attr("data-label", opts.normalText).val("no-repeat").appendTo(radios);

            radios.taoRadios({
                change: function (event, ui) {
                    var sizemode = ui.value == "cover" || ui.value == "contain" ? ui.value : "auto",
                        _val = ui.value == "cover" || ui.value == "contain" ? "no-repeat" : ui.value;
                    self.setStyle({
                        "background-repeat": _val,
                        "background-size": sizemode
                    });
                }
            });

            var btnAdd = $("<div/>").appendTo(imgButtons)
                              .text(opts.addImgText)
                              .click(function () {
                                  if (opts.netdrive) {
                                      $.fileDialog({
                                          title: "Select an image",
                                          okText: "Select",
                                          url: "/WebFiles/Explorer?locale=" + $("body").attr("lang") + "&filter=image"
                                      }).done(function (file) {
                                          if (file) {
                                              thumb.css("background-image", "url(" + file + ")");
                                              self.setStyle({ "background-image": "url(" + file + ")" });
                                          }
                                      });
                                  }
                              })
                              .taoButton();

            if (!opts.netdrive) {
                var file = $("<input type='file' size='1'/>").css({
                    "position": "absolute",
                    "opacity": "0",
                    "left": "0px",
                    "right": "0px"
                }).appendTo(btnAdd)
                  .bind("change", function (e) {
                      var _files = e.target.files;
                      if (_files && _files.length) {
                          var reader = new FileReader();
                          reader.onload = function (loadEvent) {
                              var dat = loadEvent.target.result
                              thumb.css("background-image", "url(" + dat + ")");
                              self.setStyle({ "background-image": "url(" + dat + ")" });
                          };
                          reader.readAsDataURL(_files[0]);
                      }
                  });
            }

            //.attr("data-role", "button")

            $("<div/>").appendTo(imgButtons)
                .append($("<a/>")
                .attr("href", "javascript:void(0);")
                .text(opts.removeText)
                .click(function () {
                    thumb.css("background-image", "url(" + opts.emptyImg + ")");
                    self.setStyle({ "background-image": "none" });
                }));

            var lv = $("<ul/>").appendTo(posContainer)
                .attr("data-inline", true)
                .width(120)
                .addClass("d-bg-positions");

            $("<li/>").appendTo(lv).css("background-position", "left top").attr("data-val", "left top").attr("data-alt", "0% 0%");
            $("<li/>").appendTo(lv).css("background-position", "center top").attr("data-val", "center top").attr("data-alt", "50% 0%");
            $("<li/>").appendTo(lv).css("background-position", "right top").attr("data-val", "right top").attr("data-alt", "100% 0%");
            $("<li/>").appendTo(lv).css("background-position", "left center").attr("data-val", "left center").attr("data-alt", "0% 50%");
            $("<li/>").appendTo(lv).css("background-position", "center center").attr("data-val", "center center").attr("data-alt", "50% 50%");
            $("<li/>").appendTo(lv).css("background-position", "right center").attr("data-val", "right center").attr("data-alt", "100% 50%");
            $("<li/>").appendTo(lv).css("background-position", "left bottom").attr("data-val", "left bottom").attr("data-alt", "0% 100%");
            $("<li/>").appendTo(lv).css("background-position", "center bottom").attr("data-val", "center bottom").attr("data-alt", "50% 100%");
            $("<li/>").appendTo(lv).css("background-position", "right bottom").attr("data-val", "right bottom").attr("data-alt", "100% 100%");

            lv.taoListview({
                selectable: true,
                select: function (event, ui) {
                    var pos = "background-position";
                    self.setStyle({ pos: ui.item.css(pos) });
                }
            });

            if (opts.target) {
                lv.children("[data-val=\"" + opts.target.css("background-position") + "\"]").isActive(true);
                if (lv.children(".d-state-active").length == 0)
                    lv.children("[data-alt=\"" + opts.target.css("background-position") + "\"]").isActive(true);
            }
            else
                lv.children("[data-val=\"center center\"]").isActive(true);

            this.thumb = thumb;
            this.posList = lv;
            this.scaling = radios;
        },
        _onStyleChange: function () {
            var el = this.element, opts = this.options;
            if (opts.target) {
                var css_img = "background-image",
                    css_pos = "background-position",
                    css_size = "background-size",
                    css_repeat = "background-repeat",
                    _img = opts.target.css(css_img),
                    _pos = opts.target.css(css_pos),
                    _size = opts.target.css(css_size),
                    _repeat = opts.target.css(css_repeat);

                if (_img != "" && _img != "none")
                    this.thumb.css("background-image", _img);
                else
                    this.thumb.css("background-image", "url(" + opts.emptyImg + ")")

                this.posList.children().isActive(false);
                this.posList.children("[data-val=\"" + _pos + "\"],[data-alt=\"" + _pos + "\"]").isActive(true);
                this.scaling.taoRadios("value", _size == "cover" || _size == "contain" ? _size : _repeat);
            }

        },
        csstext: function () {
            var el = this.element, opts = this.options;
            //if (opts.target) {
            var css_img = "background-image",
                css_pos = "background-position",
                css_size = "background-size",
                css_repeat = "background-repeat",
                _img = this.thumb.css(css_img),
                _pos = this.posList.children(".d-state-active").attr("data-alt");

            var css = [];

            if (_pos)
                css.push(css_pos + ":" + _pos);

            var scaling = this.scaling.taoRadios("option", "value");

            if (scaling == "cover" || scaling == "contain") {
                css.push(css_size + ":" + scaling);
                css.push(css_repeat + ":no-repeat");
            } else {
                css.push(css_repeat + ":" + scaling);
            }

            if (!_img.match(opts.emptyImg)) {
                if (_img == "" || _img == "none")
                    css.push(css_img + ":none");
                else
                    css.push(css_img + ":" + _img);
            } else
                css.push(css_img + ":none");

            return css.join(";");
        },
        destroy: function () {
            $.dna.editorbase.prototype.destroy.call(this);
        }
    });

    $.widget("dna.shadow_editor", $.dna.editorbase, {
        options: {
            distText: "Distance",
            colorText: "Color",
            blurText: "Blur",
            sizeText: "Size",
            angleText: "Angle",
            innerShadow: "Inner shadow"
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            el.addClass("d-shadow-editor");
            $.dna.editorbase.prototype._unobtrusive.call(this);
            if (el.attr("data-text-dist")) opts.distText = el.data("text-dist");
            if (el.attr("data-text-color")) opts.colorText = el.data("text-dist");
            if (el.attr("data-text-blur")) opts.blurText = el.data("text-blur");
            if (el.attr("data-text-size")) opts.sizeText = el.data("text-size");
            if (el.attr("data-text-angle")) opts.angleText = el.data("text-angle");
            if (el.attr("data-text-inner") != undefined) opts.innerShadow = el.data("text-inner");
        },
        _initUI: function () {
            var self = this, el = this.element, opts = this.options,
                _createFieldEditor = function (name, text) {
                    var container = $("<div/>").appendTo(el),
                        _val = _shadowObj[name];
                    //_val = opts.target ? (opts.target.css(name) ? parseInt(opts.target.css(name).replace("px", "")) : 0) : 0;

                    $("<div/>").text(text).appendTo(container);

                    var _slider = $("<div/>").appendTo(container)
                         .taoSlider({
                             slide: function (event, ui) {
                                 input.val(ui.value);
                                 self[name] = ui.value;
                                 self._setShadow();
                             },
                             value: _val,
                             range: "min",
                             max: 50,
                             min: 1
                         })
                        .css("margin-left", "5px")
                        .width(160)
                        .addClass("d-inline"),

                        input = $("<input type='number' name='" + name + "' data-role='none' />").appendTo(container)
                        .addClass("d-inline")
                        .css("margin-left", "5px")
                        .width(40)
                        .val(_val)
                        .bind("change", function () {
                            var w = parseInt($(this).val());
                            _slider.taoSlider("option", "value", w);
                            self[name] = w;
                            self._setShadow();
                        })
                        .bind("focus", function () { $(this).select(); });

                    $("<span/>").addClass("d-inline").appendTo(container).text("px");
                },
                _createColorEditor = function (name, label) {
                    var container = $("<div/>").appendTo(el).css("margin-top", "10px"),
                        lb = $("<label/>").appendTo(container)
                                                    .text(label)
                                                    .addClass("d-inline"),
                        input = $("<input name='" + name + "'/>").attr("data-role", "none")
                        .width(60)
                        .val(_shadowObj.color)
                        .appendTo(container)
                        //.val(opts.target ? opts.target.css(name) : "")
                        .taoColorDropdown({
                            palette: "fa",
                            change: function (event, ui) {
                                self[name] = ui;
                                self._setShadow();
                            }
                        });

                };

            var container = $("<div/>").appendTo(el).css("text-align", "center"),
            _shadows = $("<ul/>").appendTo(container).width(140).addClass("d-inline"),
                _shadowObj = this._getShadow();
            // console.log(_shadowObj);
            $("<li/>").appendTo(_shadows).attr("data-x", "-1").attr("data-y", "-1").addClass("shadow_tl");
            $("<li/>").appendTo(_shadows).attr("data-x", "0").attr("data-y", "-1").addClass("shadow_t");
            $("<li/>").appendTo(_shadows).attr("data-x", "1").attr("data-y", "-1").addClass("shadow_tr");
            $("<li/>").appendTo(_shadows).attr("data-x", "-1").attr("data-y", "0").addClass("shadow_l");
            $("<li/>").appendTo(_shadows).attr("data-x", "0").attr("data-y", "0").addClass("shadow_c d-state-active");
            $("<li/>").appendTo(_shadows).attr("data-x", "1").attr("data-y", "0").addClass("shadow_r");
            $("<li/>").appendTo(_shadows).attr("data-x", "-1").attr("data-y", "1").addClass("shadow_bl");
            $("<li/>").appendTo(_shadows).attr("data-x", "0").attr("data-y", "1").addClass("shadow_b");
            $("<li/>").appendTo(_shadows).attr("data-x", "1").attr("data-y", "1").addClass("shadow_br");

            _shadows.taoListview({
                selectable: true,
                inline: true,
                itemClass: "shadow_item",
                select: function (event, ui) {
                    self.offsetX = parseInt(ui.item.data("x"));
                    self.offsetY = parseInt(ui.item.data("y"));
                    self._setShadow();
                }
            });

            this.shadows = _shadows;
            _createColorEditor("color", opts.colorText);
            _createFieldEditor("distance", opts.distText);
            _createFieldEditor("blur", opts.blurText);
            _createFieldEditor("size", opts.sizeText);

            var cbinset = $("<input type='checkbox' name='inset'/>").appendTo(el)
                .attr("data-label", opts.innerShadow)
                .attr("data-role", "none");

            if (_shadowObj.inset)
                cbinset.attr("checked", "checked");

            cbinset.taoCheckbox({
                change: function (event, ui) {
                    if (ui.checked)
                        self.inset = " inset";
                    else
                        self.inset = "";
                    self._setShadow();
                }
            });

            this.color = "transparent";
            this.distance = 0;
            this.blur = 0;
            this.size = 0;
            this.offsetX = 0;
            this.offsetY = 0;
            this.inset = "";
        },
        _onStyleChange: function () {
            var shadowCssText = this.element.css("box-shadow"), el = this.element;
            if (shadowCssText && shadowCssText != "none") {

                if (shadowCssText.indexOf("inset") > -1)
                    this.inset = " inset";

                args = shadowCssText.replace(" inset", "").split(" ");
                //x
                this.offsetX = parseInt(args[0]);
                this.distance = Math.abs(this.offsetX > 0 ? this.offsetX / 1 : 0);

                if (this.offsetX > 0)
                    this.offsetX = 1;

                if (this.offsetX < 0)
                    this.offsetX = -1;

                //y
                this.offsetY = parseInt(args[1]);
                if (this.distance == 0)
                    this.distance = Math.abs(this.offsetY > 0 ? this.offsetY / 1 : 0);

                if (this.offsetY > 0)
                    this.offsetY = 1;

                if (this.offsetY < 0)
                    this.offsetY = -1;

                if (args.length == 4) {
                    // no blur
                    this.blur = 0;
                    this.size = args[2];
                    this.color = args[3];
                } else {
                    this.blur = args[2];
                    this.size = args[2];
                    this.color = args[4];
                }

                this.shadows.children().isActive(false);
                this.shadows.children("[data-x=" + this.offsetX + "][data-y=" + this.offsetY + "]").isActive(true);
            }

            //Fill values to controls
            $("input[name=color]", el).taoColorDropdown("option", "color", this.color);
            $("input[name=blur]", el).val(this.blur).prev().taoSlider("option", "value", this.blur);
            $("input[name=size]", el).val(this.size).prev().taoSlider("option", "value", this.size);
            $("input[name=distance]", el).val(this.distance).prev().taoSlider("option", "value", this.distance);
            $("input[name=inset]").taoCheckbox("check", this.inset != "" && this.inset != null);
            return el;
        },
        csstext: function () {
            var cssArgs = [], _cssObj = this._getCssObject();
            cssArgs.push("box-shadow:" + _cssObj["box-shadow"]);
            cssArgs.push("-webkit-box-shadow:" + _cssObj["-webkit-box-shadow"]);
            cssArgs.push("-moz-box-shadow:" + _cssObj["-moz-box-shadow"]);
            return cssArgs.join(";");
        },
        _setShadow: function () {
            this.setStyle(this._getCssObject());
        },
        _getShadowCssText: function () {
            var x = this.offsetX * this.distance,
                y = this.offsetY * this.distance,
                cssText = x + "px " + y + "px " + this.blur + "px" + (this.size ? (" " + this.size + "px") : "") + " " + this.color + this.inset;

            //+ $.hex2rgb(this.color, this.opacity / 100);
            if (this.size == 0 && this.blur == 0)
                return "none";
            return cssText;
        },
        _getCssObject: function () {
            var self = this, el = this.element, csstext = this._getShadowCssText();
            return {
                "box-shadow": csstext,
                "-webkit-box-shadow": csstext,
                "-moz-box-shadow": csstext
            }
        },
        _getShadow: function () {
            var _target = this.options.target, _shadow = {
                color: "",
                distance: 0,
                blur: 0,
                size: 0,
                offsetX: 0,
                offsetY: 0,
                inset: ""
            };

            var shadowText = _target.css("box-shadow");

            if (!shadowText)
                shadowText = _target.css("-moz-box-shadow");

            if (!shadowText)
                shadowText = _target.css("-webkit-box-shadow");

            if (shadowText && shadowText != "none") {
                //console.log(shadowText);

                var _getVal = function (index) {
                    return shadowArgs[index] ? parseInt(shadowArgs[index].replace("px", "")) : 0;
                };

                if (shadowText.indexOf("inset") > -1)
                    _shadow.inset = "inset";
                var shadowArgs = shadowText.split(" ");

                if (shadowText.startsWith("rgb")) {
                    var ends = shadowText.indexOf(")");
                    _shadow.color = shadowText.substr(0, ends + 1);
                    shadowText = shadowText.substr(ends + 2);
                    shadowArgs = shadowText.split(" ");
                    _shadow.offsetX = _getVal(0);
                    _shadow.offsetY = _getVal(1);
                    _shadow.blur = _getVal(2);
                    _shadow.size = _getVal(3);
                } else {
                    _shadow.color = shadowArgs[0];
                    _shadow.offsetX = _getVal(1);
                    _shadow.offsetY = _getVal(2);
                    _shadow.blur = _getVal(3);
                    _shadow.size = _getVal(4);
                }
                //console.log(_shadow);
            }
            return _shadow;
        },
        destroy: function () {
            $.dna.editorbase.prototype.destroy.call(this);
        }
    });

    $.widget("dna.textformat_toolbar", $.dna.editorbase, {
        options: {
            fontstyle: true,
            transform: true,
            textalign: false
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            el.addClass("d-shadow-editor");
            if (el.attr("data-fontstyle") != undefined) opts.fontstyle = el.data("fontstyle");
            if (el.attr("data-transform") != undefined) opts.transform = el.data("transform");
            if (el.attr("data-textalign") != undefined) opts.textalign = el.data("textalign");

            $.dna.editorbase.prototype._unobtrusive.call(this);
        },
        _initUI: function () {
            var self = this, el = this.element, opts = this.options,
                toolbar = $("<ul/>").appendTo(el);

            if (opts.fontstyle) {
                $("<li/>").appendTo(toolbar)
                    .attr("data-role", "checkbox")
                    .attr("data-style", "bold")
                    .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-bold")));

                $("<li/>").appendTo(toolbar)
                    .attr("data-role", "checkbox")
                    .attr("data-style", "italic")
                    .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-italic")));

                $("<li/>").appendTo(toolbar).attr("data-role", "checkbox")
                    .attr("data-style", "underline")
                              .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-underline")));
            }

            if (opts.transform) {
                if (toolbar.children().length > 0)
                    $("<li/>").addClass("d-separator").appendTo(toolbar);

                $("<li/>").appendTo(toolbar)
                    .attr("data-group", "transform")
                    .attr("data-role", "radio")
                    .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-normal-text")))
                  .attr("data-val", "none");

                $("<li/>").appendTo(toolbar)
                    .attr("data-group", "transform")
                    .attr("data-role", "radio")
                  .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-uppercase")))
                  .attr("data-val", "uppercase");

                $("<li/>").appendTo(toolbar)
                    .attr("data-group", "transform")
                    .attr("data-role", "radio")
                  .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-lowercase")))
                  .attr("data-val", "lowercase");
            }

            if (opts.textalign) {
                if (toolbar.children().length > 0)
                    $("<li/>").addClass("d-separator").appendTo(toolbar);
                $("<li/>").appendTo(toolbar)
                              .attr("data-group", "textalign")
                              .attr("data-role", "radio")
                              .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-justifyLeft")))
                              .attr("data-val", "left").attr("data-group", "text-align");

                $("<li/>").appendTo(toolbar)
                              .attr("data-group", "textalign")
                              .attr("data-role", "radio")
                              .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-justifyCenter")))
                              .attr("data-val", "center").attr("data-group", "text-align");

                $("<li/>").appendTo(toolbar)
                              .attr("data-group", "textalign")
                              .attr("data-role", "radio")
                              .append($("<a/>").attr("href", "javascript:void(0);").append($("<span/>").addClass("d-item-icon d-icon-justifyRight")))
                              .attr("data-val", "right").attr("data-group", "text-align");
            }

            toolbar.taoMenu({
                type: "toolbar",
                itemClick: function (event, ui) {
                    if (ui.item.data("group") == "transform") {
                        self._transform(ui.item.data("val"));
                        return;
                    }

                    if (ui.item.data("group") == "textalign") {
                        self._justify(ui.item.data("val"));
                        return;
                    }

                    if (ui.item.data("style")) {
                        var _style = ui.item.data("style"),
                            _val = ui.item.attr("data-checked") ? true : false;
                        if (_style == "bold")
                            self._setBold(_val);
                        if (_style == "italic")
                            self._setItalic(_val);
                        if (_style == "underline")
                            self._setUnderline(_val);
                    }
                }
            });
        },
        _justify: function (val) {
            var target = this.options.target;
            this.setStyle({ "text-align": val });
        },
        _transform: function (val) {
            var target = this.options.target;
            this.setStyle({ "text-transform": val });
        },
        _setBold: function (val) {
            var target = this.options.target;
            this.setStyle({ "font-weight": val ? "bold" : "normal" });
        },
        _setItalic: function (val) {
            var target = this.options.target;
            this.setStyle({ "font-style": val ? "italic" : "normal" });
        },
        _setUnderline: function (val) {
            var target = this.options.target;
            this.setStyle({ "text-decoration": val ? "underline" : "none" });
        },
        _onStyleChange: function () {
            var el = this.element, opts = this.options, _target = opts.target;
            if (_target && _target.length) {
                if (opts.fontstyle) {
                    if (_target.css("font-weight") > 400)
                        $("[data-style=bold]", el).attr("data-checked", "true").isActive(true);
                    if (_target.css("font-style") == "italic")
                        $("[data-style=italic]", el).attr("data-checked", "true").isActive(true);
                    if (_target.css("text-decoration") == "underline")
                        $("[data-style=underline]", el).attr("data-checked", "true").isActive(true);
                }

                if (opts.transform) {
                    var tran = _target.css("text-transform");
                    $("li[data-group=transform][data-val=" + tran + "]", el).attr("data-checked", "true").isActive(true);
                }

                if (opts.textalign) {
                    var justfy = _target.css("text-decoration");
                    $("li[data-group=textalign][data-val=" + justfy + "]", el).attr("data-checked", "true").isActive(true);
                }
            }

        },
        csstext: function () {
            var el = this.element, lines = [];

            if ($("[data-style=bold]", el).length)
                lines.push("font-weight:" + ($("[data-style=bold]", el).attr("data-checked") ? "bold" : "normal"));

            if ($("[data-style=italic][data-checked=true]", el).length)
                lines.push("font-style:italic");

            if ($("[data-style=underline][data-checked=true]", el).length)
                lines.push("text-decoration:underline");
            else
                lines.push("text-decoration:none");

            if ($("li[data-group=transform][data-checked=true]", el).length)
                lines.push("text-transform:" + $("li[data-group=transform][data-checked=true]", el).data("val"));

            if ($("li[data-group=textalign][data-checked=true]", el).length)
                lines.push("text-align:" + $("li[data-group=textalign][data-checked=true]", el).data("val"));

            return lines.join(";");
        }
    });

    $.widget("dna.fontstyles", $.dna.editorbase, {
        _initUI: function () {
            var self = this, el = this.element, opts = this.options;

            el.bind("change", function () {
                if (opts.target) {
                    self._unbindChange();
                    $("option", el).each(function (i, n) {
                        opts.target.removeClass($(n).attr("value"));
                    });
                    self._bindChange();
                    opts.target.addClass(el.val());
                }
            });
        },
        _onStyleChange: function () {
            var self = this, el = this.element, opts = this.options;
            if (opts.target) {
                $("option", el).each(function (i, n) {
                    if (opts.target.hasClass($(n).attr("value"))) {
                        el.val($(n).attr("value"));
                        return;
                    }
                });
            }
        }
    });

    $.widget("dna.value_editor", $.dna.editorbase, {
        options: {
            text: "",
            prop: "",
            unit: "px",
            empty: "none",
            max: 50
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            $.dna.editorbase.prototype._unobtrusive.call(this);
            el.addClass("d-val-editor");
            if (el.attr("data-text")) opts.text = el.data("text");
            if (el.attr("data-prop")) opts.prop = el.data("prop");
            if (el.attr("data-unit") != undefined) opts.unit = el.data("unit");
            if (el.attr("data-empty") != undefined) opts.empty = el.data("empty");
            if (el.attr("data-max")) opts.max = el.dataInt("max");
        },
        _initUI: function () {
            var self = this, el = this.element,
                opts = this.options,
                name = this.options.prop,
                _getunit = function () {
                    //var _o = opts.target.css(name);
                    //if (_o)
                    //    opts.unit = _o.substr(_o.length - 2);
                    return opts.unit;
                },
                unit = _getunit(),
                text = this.options.text,
                container = $("<div/>").appendTo(el),
                    _val = opts.target ? (opts.target.css(name) ? parseInt(opts.target.css(name).replace(unit, "")) : 0) : 0;
            $("<div/>").text(text).appendTo(container);

            var _slider = $("<div/>").appendTo(container)
                 .taoSlider({
                     slide: function (event, ui) {
                         input.val(ui.value);
                         var styleObj = {};
                         styleObj[name] = ui.value + unit;
                         self.setStyle(styleObj);
                     },
                     value: _val,
                     range: "min",
                     max: opts.max,
                     min: 1
                 })
                .css("margin-left", "5px")
                .width(160)
                .addClass("d-inline"),

                input = $("<input type='number' name='" + name + "' data-role='none' />").appendTo(container)
                .addClass("d-inline")
                .css("margin-left", "5px")
                .width(40)
                .val(_val)
                .bind("change", function () {
                    var w = parseInt($(this).val());
                    _slider.taoSlider("option", "value", w);
                    var styleObj = {};
                    styleObj[name] = w + unit;
                    self.setStyle(styleObj);

                    if ($(this).val() == "")
                        _slider.taoSlider("option", "value", opts.target ? (opts.target.css(name) ? parseInt(opts.target.css(name).replace(unit, "")) : 0) : 0);
                })
                .bind("focus", function () { $(this).select(); });

            if (opts.unit)
                $("<span/>").addClass("d-inline").appendTo(container).text("px");

            this._input = input;
            this._slider = _slider;
        },
        _onStyleChange: function () {
            if (this.options.target && this.options.prop) {
                var _valStr = this.options.target.css(this.options.prop),
                _val = _valStr ? parseInt(this.options.unit ? _valStr.replace(this.options.unit, "") : _valStr) : 0;
                this._input.val(_val);
                this._slider.taoSlider("option", "value", _val);
            }

            return this.element;
        },
        csstext: function () {
            if (this._input.val()) {
                if (this.options.unit == "em")
                    return this.options.prop + ":" + (parseInt(this._input.val()) / 16) + "em";
                else {
                    var val = this._input.val();
                    if (this.options.prop == "opacity") {
                        //val = parseInt(val) / 100;
                        return this.options.prop + ":" + parseInt(val) / 100 + ";filter:Alpha(Opacity=" + val + ");-ms-filter: 'progid:DXImageTransform.Microsoft.Alpha(Opacity=" + val + ")';";
                    } else {
                        return this.options.prop + ":" + val + this.options.unit;
                    }
                }
            }
            else {
                if (this.options.empty)
                    return this.options.prop + ":" + this.options.empty;
                else
                    return "";
            }
        }
    });

    $.widget("dna.prop_selector", $.dna.editorbase, {
        options: {
            text: "",
            prop: "",
            empty: "none"
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            $.dna.editorbase.prototype._unobtrusive.call(this);
            el.addClass("d-prop-selector");
            if (el.attr("data-prop")) opts.prop = el.data("prop");
            if (el.attr("data-target-selector"))
                opts.target = el.datajQuery("target-selector");
        },
        _initUI: function () {
            var self = this, _target = this.options.target, opts = this.options;
            _target.bind("taoComboBoxchange", function (event, ui) {
                var obj = {};
                obj[opts.prop] = ui.value;
                self.setStyle(obj);
            });
        },
        _onStyleChange: function () {
            var opts = this.options, target = opts.target,
                el = this.element,
                val = target.css(opts.prop);
            //if (el.taoComboBox("find", "'" + val + "'").length == 0)
            //el.taoComboBox("clear");
        },
        csstext: function () {
            var val = this.element.taoComboBox("option", "value");
            if (val)
                return this.options.prop + ":" + val;
            return "";
        }
    });

    $.widget("dna.textshadow_editor", $.dna.editorbase, {
        options: {
            blurText: "Blur",
            colorText: "Color",
            distText: "Distance"
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            $.dna.editorbase.prototype._unobtrusive.call(this);
            el.addClass("d-textshadow-editor");
            if (el.attr("data-text-blur")) opts.blurText = el.data("text-blur");
            if (el.attr("data-text-color")) opts.colorText = el.data("text-color");
            if (el.attr("data-text-dist")) opts.distText = el.data("text-dist");
        },
        _initUI: function () {
            var self = this, el = this.element, opts = this.options,
                _createFieldEditor = function (name, text) {
                    var container = $("<div/>").appendTo(el),
                    _val = opts.target ? (opts.target.css(name) ? parseInt(opts.target.css(name).replace("px", "")) : 0) : 0;
                    $("<div/>").text(text).appendTo(container);

                    var _slider = $("<div/>").appendTo(container)
                         .taoSlider({
                             slide: function (event, ui) {
                                 input.val(ui.value);
                                 self[name] = ui.value;
                                 self._setShadow();
                             },
                             value: _val,
                             range: "min",
                             max: 50,
                             min: 1
                         })
                        .css("margin-left", "5px")
                        .width(160)
                        .addClass("d-inline"),

                        input = $("<input type='number' name='" + name + "' data-role='none' />").appendTo(container)
                        .addClass("d-inline")
                        .css("margin-left", "5px")
                        .width(40)
                        .val(_val)
                        .bind("change", function () {
                            var w = parseInt($(this).val());
                            _slider.taoSlider("option", "value", w);
                            self[name] = w;
                            self._setShadow();
                        })
                        .bind("focus", function () { $(this).select(); });

                    $("<span/>").addClass("d-inline").appendTo(container).text("px");
                },
                _createColorEditor = function (name, label) {
                    var container = $("<div/>").appendTo(el).css("margin-top", "10px"),
                        lb = $("<label/>").appendTo(container)
                                                    .text(label)
                                                    .addClass("d-inline"),
                        input = $("<input name='" + name + "'/>").attr("data-role", "none")
                        .width(60)
                        .appendTo(container)
                        //.val(opts.target ? opts.target.css(name) : "")
                        .taoColorDropdown({
                            palette: "fa",
                            change: function (event, ui) {
                                self[name] = ui;
                                self._setShadow();
                            }
                        });

                },
             container = $("<div/>").appendTo(el).css("text-align", "center"),
            _shadows = $("<ul/>").appendTo(container).width(140).addClass("d-inline");
            $("<li/>").appendTo(_shadows).attr("data-x", "-1").attr("data-y", "-1").addClass("text_shadow_tl").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "0").attr("data-y", "1").addClass("text_shadow_t").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "1").attr("data-y", "-1").addClass("text_shadow_tr").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "-1").attr("data-y", "0").addClass("text_shadow_l").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "0").attr("data-y", "0").addClass("text_shadow_c d-state-active").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "1").attr("data-y", "0").addClass("text_shadow_r").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "-1").attr("data-y", "1").addClass("text_shadow_bl").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "0").attr("data-y", "1").addClass("text_shadow_b").text("A");
            $("<li/>").appendTo(_shadows).attr("data-x", "1").attr("data-y", "1").addClass("text_shadow_br").text("A");

            _createColorEditor("color", opts.colorText);

            _shadows.taoListview({
                selectable: true,
                inline: true,
                itemClass: "text_shadow_item d-float-left",
                select: function (event, ui) {
                    self.offsetX = parseInt(ui.item.data("x"));
                    self.offsetY = parseInt(ui.item.data("y"));
                    self._setShadow();
                }
            });
            this.shadows = _shadows;
            _createFieldEditor("distance", opts.distText);
            _createFieldEditor("blur", opts.blurText);

            this.color = "transparent";
            this.distance = 1;
            this.blur = 0;
            this.offsetX = 0;
            this.offsetY = 0;
        },
        _setShadow: function () {
            this.setStyle(this._getShadowObject());
        },
        _getShadowCssText: function () {
            var x = this.offsetX * this.distance,
                y = this.offsetY * this.distance,
                cssText = x + "px " + y + "px " + this.blur + "px " + this.color;

            //+ $.hex2rgb(this.color, this.opacity / 100);
            if ((this.color == "" || this.color == "transparent") && this.blur == 0)
                return "none";

            return cssText;
        },
        _getShadowObject: function () {
            var self = this, el = this.element, csstext = this._getShadowCssText();
            return { "text-shadow": csstext };
        },
        _onStyleChange: function () {
            var shadowCssText = this.element.css("box-shadow"), el = this.element;

            if (shadowCssText && shadowCssText != "none") {
                var args = shadowCssText.split(" ");
                this.offsetX = parseInt(args[0].replace("px", ""));
                this.distance = Math.abs(this.offsetX > 0 ? this.offsetX / 1 : 0);

                if (this.offsetX > 0)
                    this.offsetX = 1;

                if (this.offsetX < 0)
                    this.offsetX = -1;

                this.shadows.children().isActive(false);
                this.shadows.children("[data-x=" + this.offsetX + "][data-y=" + this.offsetY + "]").isActive(true);
                this.blur = parseInt(args[2].replace("px", ""));
                this.color = args[3];

                $("input[name=color]", el).taoColorDropdown(this.color);
                $("input[name=blur]", el).val(this.blur).prev().taoSlider("option", "value", this.blur);
                $("input[name=distance]", el).val(this.distance).prev().taoSlider("option", "value", this.distance);
            }

            return el;
        },
        csstext: function () {
            var cssArgs = [], _cssObj = this._getShadowObject();
            cssArgs.push("text-shadow:" + _cssObj["text-shadow"]);
            return cssArgs.join(";");
        }
    });

    $.widget("dna.gradient_editor", $.dna.editorbase, {
        options: {
            startText: "Start color",
            endText: "End color",
            reverseText: "Reverse"
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            $.dna.editorbase.prototype._unobtrusive.call(this);
            el.addClass("d-gradient-editor");
            if (el.attr("data-text-start")) opts.startText = el.data("text-start");
            if (el.attr("data-text-end")) opts.endText = el.data("text-end");
            if (el.attr("data-text-reverse")) opts.reverseText = el.data("text-reverse");
        },
        _initUI: function () {
            var self = this, el = this.element, opts = this.options,
            _createColorEditor = function (name, label,value) {
                var container = $("<div/>").appendTo(el).css("margin-top", "10px"),
                    lb = $("<label/>").appendTo(container)
                                                .text(label)
                                                .addClass("d-inline"),
                    input = $("<input name='" + name + "' type='color'/>").attr("data-role", "none")
                    .width(80)
                    .val(value)
                    .appendTo(container)
                    .taoColorDropdown({
                        palette: "fa",
                        change: function (event, ui) {
                            self[name] = ui;
                            self._setGradient();
                        }
                    });
                return input;
            },
            _gradient=this._getGradientObj(),
            startColor = _createColorEditor("startColor", opts.startText,_gradient.starts),
            endColor = _createColorEditor("endColor", opts.endText,_gradient.ends),
            reverseContainer = $("<div/>").appendTo(el).css("margin-bottom", "10px");

            $("<input type='checkbox' />").appendTo(reverseContainer)
                                                              .attr("data-label", opts.reverseText)
                                                              .taoCheckbox({
                                                                  change: function (event, ui) {
                                                                      var _start = startColor.val(), _end = endColor.val();
                                                                      startColor.taoColorpicker("option", "color", _end);
                                                                      endColor.taoColorpicker("option", "color", _start);
                                                                      self._setGradient();
                                                                  }
                                                              });
        },
        _setGradient: function () {
            var startColor = $("input[name=startColor]", this.element).val(),
              endColor = $("input[name=endColor]", this.element).val();

            if (startColor && endColor && (startColor != endColor))
                this.setStyle(this._genGradientCss());
            else {
                var bgcolor = startColor;
                if (bgcolor == "")
                    bgcolor = endColor;

                this.setStyle({ "background-color": bgcolor });
            }
        },
        _getGradientObj: function () {
            var _target = this.options.target,
                bgImgStr = _target.css("background-image"),
                _filter = _target.css("filter"),
                    _results = { starts: "", ends: "" };
            //console.log(_target);
            //console.log(bgImgStr);

            if (bgImgStr == "none" || bgImgStr == "")
                bgImgStr = _target.css("background");

            var regex = /-gradient\((.+?)\)/;

            if (_filter && _filter.match(regex))
                bgImgStr = _filter;

            //console.log(bgImgStr);

            if ((bgImgStr && bgImgStr != "none") && bgImgStr.match(regex)) {
                //has gradient function
                var matchResult = bgImgStr.match(regex)[1], _start = "",
                    _end = "";
                if (matchResult.startsWith("linear")) {
                    //webkit 1
                    _start = matchResult.match(/color-stop\(0% (.+?)\)/);
                    _end = matchResult.match(/color-stop\(100% (.+?)\)/);
                }
                else {
                    if (matchResult.startsWith("startColorstr")) {
                        //ie
                        _start = matchResult.match(/startColorstr='(.+?)'/);
                        _end = matchResult.match(/endColorstr='(.+?)'/);
                    }
                    else {
                        //all
                        _start = matchResult.match(/,(.+?) 0%/);
                        _end = matchResult.match(/%,(.+?) 100%/);
                    }
                }

                if (_start && _start != "none") 
                    _results.starts = _start;

                if (_end && _end != "none")
                    _results.ends = _end;

            } else {
                var bgColor = _target.css("background-color");
                if (bgColor != "" && bgColor != "none") 
                    _results.ends = bgColor;
            }
            return _results;
        },
        _genGradientCss: function () {
            var startColor = $("input[name=startColor]", this.element).val(), endColor = $("input[name=endColor]", this.element).val();
            return {
                "background-image": "none,-webkit-gradient(linear, left top, left bottom, color-stop(0% " + startColor + "),color-stop(100% " + endColor + "))",
                "background-image": "none,-webkit-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)",
                "background-image": "none,-moz-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)",
                "background-image": "none,-o-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)",
                "background-image": "none,-ms-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)",
                "background-image": "none,linear-gradient(to bottom," + startColor + " 0%," + endColor + " 100%)",
                "filter": "progid:DXImageTransform.Microsoft.gradient( startColorstr='" + startColor + "', endColorstr='" + endColor + "',GradientType=0 )"
            };
        },
        _onStyleChange: function () {
            var _target = this.options.target,
                startEle = $("input[name=startColor]", this.element),
                endEle = $("input[name=endColor]", this.element),
                    _gradient = this._getGradientObj();

            if (_gradient.starts)
                startEle.taoColorDropdown("option", "color", _gradient.starts);

            if (_gradient.ends)
                endEle.taoColorDropdown("option", "color", _gradient.ends);

                //, startColor = startEle.val(),
                //endColor = endEle.val();
            //,
            //    bgImgStr = _target.css("background-image"),
            //    _filter = _target.css("filter");

            //if (bgImgStr == "none" || bgImgStr == "")
            //    bgImgStr = _target.css("background");

            //var regex = /-gradient\((.+?)\)/;

            //if (_filter && _filter.match(regex))
            //    bgImgStr = _filter;

            //if ((bgImgStr && bgImgStr != "none") && bgImgStr.match(regex)) {
            //    //has gradient function
            //    var matchResult = bgImgStr.match(regex)[1], _start = "",
            //        _end = "";
            //    if (matchResult.startsWith("linear")) {
            //        //webkit 1
            //        _start = matchResult.match(/color-stop\(0% (.+?)\)/);
            //        _end = matchResult.match(/color-stop\(100% (.+?)\)/);
            //    }
            //    else {
            //        if (matchResult.startsWith("startColorstr")) {
            //            //ie
            //            _start = matchResult.match(/startColorstr='(.+?)'/);
            //            _end = matchResult.match(/endColorstr='(.+?)'/);
            //        }
            //        else {
            //            //all
            //            _start = matchResult.match(/,(.+?) 0%/);
            //            _end = matchResult.match(/%,(.+?) 100%/);
            //        }
            //    }

            //    if (_start && _start != "none") {
            //        startEle.taoColorDropdown("option", "color", _start);
            //    }

            //    if (_end && _end != "none") {
            //        endEle.taoColorDropdown("option", "color", _end);
            //    }
            //} else {
            //    var bgColor = _target.css("background-color");
            //    if (bgColor != "" && bgColor != "none") {
            //        endEle.taoColorDropdown("option", "color", bgColor);
            //    }
            //}

        },
        csstext: function () {
            var startColor = $("input[name=startColor]", this.element).val(), endColor = $("input[name=endColor]", this.element).val();
            if (startColor && endColor) {
                var cssArgs = [];
                cssArgs.push("background-image: none,-webkit-gradient(linear, left top, left bottom, color-stop(0% " + startColor + "),color-stop(100% " + endColor + "))");
                cssArgs.push("background-image: none,-webkit-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)");
                cssArgs.push("background-image: none,-moz-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)");
                cssArgs.push("background-image: none,-o-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)");
                cssArgs.push("background-image: none,-ms-linear-gradient(top," + startColor + " 0%," + endColor + " 100%)");
                cssArgs.push("background-image: none,linear-gradient(to bottom," + startColor + " 0%," + endColor + " 100%)");
                cssArgs.push("filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='" + startColor + "', endColorstr='" + endColor + "',GradientType=0 )")
                return cssArgs.join(";");
            }
            else {
                var bgcolor = endColor;
                if (bgcolor == "")
                    bgcolor = startColor;
                return "background-image:none;background-color:" + bgcolor;
            }
        }
    });

    $.widget("dna.boxsize_editor", $.dna.editorbase, {
        options: {
            paddingText: "padding",
            marginText: "margin",
            borderText: "border",
            autoMarginText: "auto margin"
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            el.addClass("d-corners-editor");
            $.dna.editorbase.prototype._unobtrusive.call(this);
        },
        _initUI: function () {
            var el = this.element, opts = this.options, self = this,
                container = $("<div/>").appendTo(el).addClass("d-style-layout"),
                marginWrapper = $("<div/>").appendTo(container)
                                                                .addClass("d-box-margin")
                                                                .append($("<span/>").addClass("d-box-margin-text").text(opts.marginText)),
                borderWrapper = $("<div/>").appendTo(container)
                                                                .addClass("d-box-border")
                                                                .append($("<span/>").addClass("d-box-border-text").text(opts.borderText)),
                paddingWrapper = $("<div/>").appendTo(container)
                                                                  .addClass("d-box-padding")
                                                                  .append($("<span/>").addClass("d-box-padding-text").text(opts.paddingText)),
                coreWrapper = $("<div/>").appendTo(container)
                                                             .addClass("d-box-core"),
            _creatInput = function (_parent, _prop) {
                var input = $("<input/>").appendTo(_parent)
                                      .attr("name", _prop)
                                      .attr("data-role", "none")
                                      .attr("data-prop", _prop)
                                      .addClass("d-" + _prop)
                                      .bind("focus", function () { $(this).select(); })
                                      .bind("change", function () {
                                          var obj = {};
                                          obj[_prop] = (parseInt($(this).val()) / 16) + "em";
                                          $(this).attr("data-changed", "true");
                                          self.setStyle(obj);
                                      })
                                      .bind("keypress", function (e) {
                                          if (e.keyCode == 13) {
                                              if ($(this).next().length)
                                                  $(this).next().focus();
                                              else
                                                  $(this).siblings("input:first").focus();
                                          }
                                      });
                var _val = opts.target ? (opts.target.css(name) ? parseInt(opts.target.css(name).replace("px", "")) : 0) : 0;
                input.val(_val);
            };

            _creatInput(marginWrapper, "margin-top");
            _creatInput(marginWrapper, "margin-left");
            _creatInput(marginWrapper, "margin-bottom");
            _creatInput(marginWrapper, "margin-right");

            _creatInput(borderWrapper, "border-top-width");
            _creatInput(borderWrapper, "border-left-width");
            _creatInput(borderWrapper, "border-bottom-width");
            _creatInput(borderWrapper, "border-right-width");

            _creatInput(paddingWrapper, "padding-top");
            _creatInput(paddingWrapper, "padding-left");
            _creatInput(paddingWrapper, "padding-bottom");
            _creatInput(paddingWrapper, "padding-right");

        },
        _onStyleChange: function () {
            var opts = this.options;
            $("input", this.element).each(function (i, propInput) {
                var _input = $(propInput), name = _input.attr("name"),
                _val = opts.target ? (opts.target.css(name) ? parseInt(opts.target.css(name).replace("px", "")) : 0) : 0;
                _input.val(_val);
            });
        },
        csstext: function () {
            var lines = [], el = this.element, self = this;
            $("input[data-changed]", el).each(function (i, propInput) {
                lines.push($(propInput).attr("name") + ":" + (parseInt($(propInput).val()) / 16) + "em");
            });

            if (lines.length)
                return lines.join(";");

            return "";
        }
    });

    $.fn.unobtrusive_editors = function () {
        $("[data-role=corners_editor]", this).corners_editor();
        $("[data-role=colors_editor]", this).colors_editor();
        $("[data-role=gradient_editor]", this).gradient_editor();
        $("[data-role=background_editor]", this).background_editor();
        $("[data-role=shadow_editor]", this).shadow_editor();
        $("[data-role=textformat_toolbar]", this).textformat_toolbar();
        $("[data-role=fontstyles]", this).fontstyles();
        $("[data-role=textshadow_editor]", this).textshadow_editor();
        $("[data-role=value_editor]", this).value_editor();
        $("[data-role=boxsize_editor]", this).boxsize_editor();
        $("[data-prop-selector=true]", this).prop_selector();
    }

})(jQuery);