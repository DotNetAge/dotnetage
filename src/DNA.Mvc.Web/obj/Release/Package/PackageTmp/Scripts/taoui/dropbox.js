(function ($) {
    $.widget("dna.taoDropbox", {
        options: {
            //thumbHeight: 0,
            //thumbWidth: 0,
            imgSize: null,
            read: null,
            emptytext: "Drop image here"
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();

            var box = $("<div/>").addClass("d-reset d-dropbox").appendTo("body");
            el.before(box);

            var _emptyText = $("<span/>").text(opts.emptytext).appendTo(box),
            closer = $("<div/>").addClass("d-icon-cross-3")
                              .appendTo(box)
                              .click(function () {
                                  el.val("").trigger("change");
                              });

            el.bind("change", function () {
                if ($(this).val()) {
                    box.css("background-image", "url(" + $(this).val() + ")");
                    _emptyText.hide();
                    closer.show();
                } else {
                    box.css("background-image", "none");
                    _emptyText.show();
                    closer.hide();
                }
            });

            if (opts.read)
                el.bind(eventPrefix + "read", opts.read);

            if (el.attr("style"))
                box.attr("style", el.attr("style"));

            el.hide().appendTo(box);
            self.dropbox = box;

            if (box[0].addEventListener) {
                box[0].addEventListener('dragenter', function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                }, false);

                box[0].addEventListener('dragover', function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    box.isHover(true);
                }, false);

                box[0].addEventListener('dragleave', function (e) {
                    e.stopPropagation();
                    e.preventDefault();
                    box.isHover(false);
                }, false);

                box[0].addEventListener('drop', $.proxy(self._ondrop, self), false);

            } else {
                _emptyText.remove();
            }

            if (opts.imgSize) {
                box.css({
                    "background-size": opts.imgSize
                });
            }

            var offsetW = box.outerWidth(true) - box.width(),
                offsetH = box.outerHeight(true) - box.height();

            if (el.val()) {
                box.css({
                    "background-image": "url(" + el.val() + ")",
                    "background-size": "contain"
                });
                _emptyText.hide();
                closer.show();
            } else {
                // _emptyText.css("line-height", el.height() + "px");
                closer.hide();
            }

        },
        _unobtrusive: function () {
            var self = this, opts = this.options, el = this.element;
            if (el.data("img-size")) opts.imgSize = el.data("img-size");
            //if (el.data("thumb-height")) opts.thumbHeight = el.dataInt("thumb-height");
            //if (el.data("thumb-width")) opts.thumbWidth = el.dataInt("thumb-width");
            if (el.data("empty-text")) opts.emptytext = el.data("empty-text");
            if (el.data("read")) opts.read = new Function("event", "ui", el.data("read"));
        },
        _ondrop: function (e) {
            var self = this, opts = this.options, el = this.element, dropbox = self.dropbox;
            e.stopPropagation();
            e.preventDefault();
            dropbox.isHover(false);

            var readFileSize = 0;
            var files = e.dataTransfer.files;
            // Loop through list of files user dropped.
            var file = files[0];
            //for (var i = 0, file; file = files[i]; i++) {
            //readFileSize += file.fileSize;
            // Only process image files.
            var imageType = /image.*/;
            if (!file.type.match(imageType)) { return; }

            var reader = new FileReader();
            reader.onerror = function (e) {
                alert('Error code: ' + e.target.error.code);
            };

            reader.onload = (function (aFile) {
                return function (evt) {
                    //dropbox.css("background-image", "url(" + evt.target.result + ")").children().hide();
                    el.val(evt.target.result);
                    //console.log(evt);
                    self._triggerEvent("read", { data: evt.target.result, size: evt.loaded, fileReader: evt.target });
                    el.trigger("change");
                };
            })(file);
            reader.readAsDataURL(file);
            //totalFileSize += readFileSize;
            return false;
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
        //        _setOption: function (key, value) {
        //            return $.Widget.prototype._setOption.call(this, key, value);
        //        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);