/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {

    $.widget("dna.taoUploader", {
        options: {
            url: "",
            data: null,
            refresh: 300,
            connectTo: null, //Set the uploadinfo widget to show the upload information
            fieldName: "userfile",
            autoblocking: false,
            fileName: null, //rename the file to new name only avaliable for single file upload
            multi: true,
            fileInput: null, //Set the file input that allows the user select the files by click file input.
            start: null,
            beforeUpload: null,
            complete: null,
            progress: null,
            speedUpdated: null,
            uploaded: null,
            dragover: null,
            dragleave: null,
            //  drop: null,
            maxSize: 1, //M
            maxCount: 20,
            error: null,
            async: true
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            if (opts.complete)
                el.bind(eventPrefix + "complete", opts.complete);

            if (opts.beforeUpload)
                el.bind(eventPrefix + "beforeUpload", opts.beforeUpload);

            if (opts.uploaded)
                el.bind(eventPrefix + "uploaded", opts.uploaded);

            if (opts.progress)
                el.bind(eventPrefix + "progress", opts.progress);

            if (opts.error)
                el.bind(eventPrefix + "error", opts.error);

            if (opts.start)
                el.bind(eventPrefix + "start", opts.start);

            if (opts.dragover)
                el.bind(eventPrefix + "dragover", opts.dragover);

            //            if (opts.drop)
            //                el.bind(eventPrefix + "drop", opts.drop);

            if (opts.dragleave)
                el.bind(eventPrefix + "dragleave", opts.dragleave);

            if (opts.speedUpdated)
                el.bind(eventPrefix + "speedUpdated", opts.speedUpdated);

            el.get(0).addEventListener("dragover", $.proxy(self._onDragOver, self), false);
            el.get(0).addEventListener("dragleave", $.proxy(self._onDragLeave, self), false);
            el.get(0).addEventListener("drop", $.proxy(self._onDrag, self), false);

            if (opts.fileInput)
                opts.fileInput.bind("change", $.proxy(this._onDrag, this));

            this._setFileInput(opts.fileInput);

            if (el.get(0).tagName.toLowerCase() == "input") {
                var _type = el.attr("type");
                if (_type) {
                    if (_type == "file") {
                        opts.multi = false;
                        opts.fileInput = el;
                        el.wrap("<div class='d-file'></div>");
                        var wrapper = el.parent(),
                        _fileHolder = $("<div class='d-input'/>").appendTo(wrapper),
                        _btn = $("<div/>").appendTo(wrapper);

                        if (el.attr("style"))
                        {
                            _fileHolder.attr("style", el.attr("style"));
                            el.removeAttr("style");
                        }

                        _btn.text("BROWSE");
                        _btn.taoButton();
                        _fileHolder.text(el.val());
                        el.appendTo(_btn).bind("change", function () {
                            _fileHolder.text($(this).val());
                        }).bind("change", $.proxy(this._onDrag, this));

                        el.attr("size", 1);
                        self._setFileInput(el);
                        _fileHolder.click(function () { el.click(); });
                    }
                }
            }


        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("url")) opts.url = el.data("url");
            if (el.data("refresh")) opts.refresh = el.dataInt("refresh");
            if (el.data("name")) opts.fieldName = el.data("name");
            if (el.data("multi") != undefined) opts.multi = el.dataBool("multi");
            if (el.data("async") != undefined) opts.async = el.dataBool("async");
            if (el.data("autoblocking") != undefined) opts.autoblocking = el.dataBool("autoblocking");
            if (el.data("filename")) opts.fileName = el.data("filename");
            if (el.data("max")) opts.maxSize = el.dataInt("max");
            if (el.data("max-count")) opts.maxCount = el.dataInt("max-count");
            if (el.data("onbeforeupload")) opts.beforeUpload = new Function("event", "files", el.data("onbeforeupload"));
            if (el.data("oncomplete")) opts.complete = new Function("event", "count", el.data("oncomplete"));
            if (el.data("onuploaded")) opts.uploaded = new Function("event", "data", "xhr", el.data("onuploaded"));
            if (el.data("onprogress")) opts.progress = new Function("event", "data", el.data("onprogress"));
            if (el.data("onrefresh")) opts.speedUpdated = new Function("event", "data", el.data("onrefresh"));
            if (el.data("onstart")) opts.start = new Function("event", "data", el.data("onstart"));
            if (el.data("onerror")) opts.error = new Function("event", "error", el.data("onerror"));
            if (el.data("files")) opts.fileInput = el.datajQuery("files");
            if (el.data("connect")) opts.connectTo = el.datajQuery("connect");
        },
        _setFileInput: function (input) {
            if (input) {
                input.bind("change", $.proxy(this._upload, this));
                this.options.fileInput = input;
            }
            return this;
        },
        _onDragLeave: function (evt) {
            //$(evt.target).css("border", "1px solid transparent");
            this._triggerEvent("dragleave");
        },
        _onDragOver: function (evt) {
            evt.stopPropagation();
            evt.preventDefault();
            evt.dataTransfer.dropEffect = "copy"; // Explicitly show this is a copy.
            this._triggerEvent("dragover");
            //console.log(evt);
            //$(evt.target).css("border","5px solid #efefef");
        },
        _getBuilder: function (filename, filedata, boundary) {
            var dashdash = '--', opts = this.options,
        crlf = '\r\n',
        builder = '';
            if (opts.data) {
                for (var p in opts.data) {
                    var _val;

                    if ($.isFunction(opts.data[p]))
                        _val = opts.data[p]();
                    else
                        _val = opts.data[p];

                    builder += dashdash;
                    builder += boundary;
                    builder += crlf;
                    builder += 'Content-Disposition: form-data; name="' + p + '"';
                    builder += crlf;
                    builder += crlf;
                    builder += _val;
                    builder += crlf;
                }
            }

            builder += dashdash;
            builder += boundary;
            builder += crlf;
            builder += "Content-Disposition: form-data; name=\"" + opts.fieldName + "\"";
            builder += '; filename="' + filename + '"';
            builder += crlf;

            builder += 'Content-Type: application/octet-stream';
            builder += crlf;
            builder += crlf;

            builder += filedata;
            builder += crlf;

            builder += dashdash;
            builder += boundary;
            builder += dashdash;
            builder += crlf;
            return builder;
        },
        _progress: function (e) {
            var self = this.context.widget, opts = this.context.options;

            if (e.lengthComputable) {
                var percentage = Math.round((e.loaded * 100) / e.total);
                if (this.currentProgress != percentage) {
                    this.currentProgress = percentage;

                    if (opts.connectTo)
                        $(opts.connectTo).taoUploadInfo("setInfo", { index: this.index, count: self.uploadCount, progress: this.currentProgress });

                    self._triggerEvent("progress", { index: this.index, file: this.file, progress: this.currentProgress });
                    var elapsed = new Date().getTime();
                    var diffTime = elapsed - this.currentStart;
                    if (diffTime >= opts.refresh) {
                        var diffData = e.loaded - this.startData;
                        var speed = diffData / diffTime; // KB per second
                        self._triggerEvent("speedUpdated", { index: this.index, file: this.file, speed: speed });
                        this.startData = e.loaded;
                        this.currentStart = elapsed;
                    }
                }
            }
        },
        _onDrag: function (event) {
            var self = this, opts = this.options, el = this.element;
            event.stopPropagation();
            event.preventDefault();

            if (el.isDisable())
                return this;

            if (!opts.url)
                return this;

            var data = event.dataTransfer,
            files = null

            _maxSize = opts.maxSize * 1048576;

            if (data)
                files = data.files;
            else {
                if (event.target.files)
                    files = event.target.files;
            }

            if (files == null)
                return this;

            if (files.length > opts.maxCount) {
                $.err("Each time you can only upload " + opts.maxCount + " files.");
                return;
            }

            var beforeEvt = $.Event(this.widgetEventPrefix + "beforeUpload");
            this.element.trigger(beforeEvt, { files: files });

            if (beforeEvt.result == false) {
                return;
            }

            this.uploadCount = files.length;

            if (opts.connectTo)
                $(opts.connectTo).taoUploadInfo("setInfo", { index: 0, count: files.length });
            else
                this.block();

            this.upload(files);

            return this;
        },
        upload: function (files) {
            ///<summary>Upload files with Html5 FileAPI</summary>
            var self = this, opts = this.options;
            var boundary = '------multipartformboundary' + (new Date).getTime(),
              _context = {
                  index: -1,
                  files: [],
                  current: function () {
                      return this.files[this.index];
                  }
              };
            _next = function () {
                _context.index++;
                if (_context.index < _context.files.length) {
                    var reader = new FileReader();
                    reader.index = _context.index;
                    reader.onloadend = _send;
                    reader.readAsBinaryString(_context.current());
                }
            };
            _send = function (evt) {
                var dashdash = '--',
                crlf = '\r\n',
                xhr = new XMLHttpRequest(),
                upload = xhr.upload,
                file = files[evt.target.index],
                index = evt.target.index,
                start_time = new Date().getTime(), _fileName = file.name;
                //console.log(file);

                if (opts.fileName) {
                    if (opts.multi) {
                        if ($.isFunction(opts.fileName))
                            _fileName = opts.fileName(index, file);
                    }
                    else
                        _fileName = opts.fileName;
                }

                var builder = self._getBuilder(encodeURIComponent(_fileName), evt.target.result, boundary);

                upload.index = index;
                upload.file = file;
                upload.downloadStartTime = start_time;
                upload.currentStart = start_time;
                upload.currentProgress = 0;
                upload.startData = 0;

                upload.context = {
                    widget: self,
                    element: self.element,
                    options: opts
                };

                upload.addEventListener("progress", $.proxy(self._progress, upload), false);

                xhr.open("POST", opts.url, true);
                xhr.setRequestHeader('content-type', 'multipart/form-data; boundary=' + boundary);
                self._triggerEvent("start", { index: index, file: file, count: files.length });

                xhr.onload = function (event) {

                    if (xhr.status >= 400) {
                        self.unBlock();
                        self._triggerEvent("error", { file: file, error: xhr.statusText });
                        return;
                    }

                    /* If we got an error display it. */
                    var result = xhr.responseText;
                    if (result) {
                        if (xhr.responseType == "json") {
                            if (result) {
                                result = $.parseJSON(result);
                                if (result.error) {
                                    self.unBlock();
                                    self._triggerEvent("error", { file: file, error: result.error });
                                    return;
                                }
                            }
                        }
                    }



                    self._triggerEvent("uploaded", { index: index, file: file, result: result, xhr: xhr });

                    if (index == (files.length - errCount) - 1) {
                        self.unBlock();
                        self._triggerEvent("complete", files.length);
                    }

                    if (!opts.async)
                        _next();


                };

                xhr.onerror = function (event) {
                    self.unBlock();
                    self._triggerEvent("error", { file: file, error: event });
                }

                xhr.sendAsBinary(builder);
            };

            var errCount = 0;

            if (opts.async) {
                //Async mode

                /* For each dropped file. */
                for (var i = 0; i < files.length; i++) {
                    if (!opts.multi && i > 0)
                        break;

                    var file = files[i];

                    if (_maxSize > 0 && file.size > _maxSize) {
                        self._triggerEvent("error", { file: file, error: "The file \"" + file.name + "\" size must less then " + (_maxSize / 1048576) + "M. This file will be skipped." });
                        errCount++;
                        continue;
                    }

                    var reader = new FileReader();
                    reader.index = i;
                    reader.onloadend = _send;
                    reader.readAsBinaryString(files[i]);
                }

            } else {
                //Seq mode

                _context.files = [];
                _context.index = -1;

                /* For each dropped file. */
                for (var i = 0; i < files.length; i++) {
                    if (!opts.multi && i > 0)
                        break;

                    var file = files[i];

                    if (_maxSize > 0 && file.size > _maxSize) {
                        self._triggerEvent("error", { file: file, error: "The file \"" + file.name + "\" size must less then " + (_maxSize / 1048576) + "M. This file will be skipped." });
                        errCount++;
                        continue;
                    }
                    _context.files.push(file);
                }

                if (_context.files.length)
                    _next();
            }

            if (errCount == files.length) {
                self.unBlock();
                self._triggerEvent("complete", files.length);
            }
        },
        block: function () {
            if (this.options.autoblocking) {
                var uploadInfo = $("<div/>").appendTo("body").width(300).height(50);
                uploadInfo.taoUploadInfo({
                    uploader: this.element
                });
                this.element.blockUI(uploadInfo);
            }
            return this;
        },
        unBlock: function () {
            if (this.options.autoblocking)
                this.element.unblockUI();
            return this;
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
        _setOption: function (key, value) {
            if (key == "fileInput")
                return this._setFileInput(value);

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });

    $.widget("dna.taoProgressbar", {
        options: {
            value: 0,
            step: 1,
            max: 100,
            complete: null,
            change: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            el.addClass("d-ui-widget d-progress");
            el.attr("role", "progressbar");
            this.progressbar = $("<div/>").appendTo(el).addClass("d-reset d-ui-widget-content d-state-active").css("padding","0px").height(el.height());
            this.val(opts.value);
        },
        _unobtrusive: function () {
            var opts = this.options, el = this.element;
            if (el.data("max") != undefined)
                opts.max = el.dataInt("max");
            if (el.data("value") != undefined)
                opts.value = el.dataInt("value");
            if (el.data("step") != undefined)
                opts.step = el.dataInt("step");
            if (el.data("oncomplete")) opts.complete = new Function("event", el.data("oncomplete"));
            if (el.data("onchange")) opts.change = new Function("event", "value", el.data("onchange"));
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        disable: function () {
            this.widget().addClass("d-state-disable");
            return this;
        },
        enable: function () {
            this.widget().removeClass("d-state-disable");
            return this;
        },
        getPercentage: function () {
            var opts = this.options;
            var returns = ((opts.step / opts.max) * 100) * opts.value;
            if (isNaN(returns))
                return 0;
            else
                return returns;
        },
        val: function (val) {
            var self = this, opts = this.options;
            if (val != undefined) {
                w = ((opts.step / opts.max) * 100) * val;
                if (isNaN(w))
                    w=0;
                this.progressbar.css({ "width": w + "%" });

                this.options.value = val;
                this._triggerEvent("change", val);
                if (val == 100)
                    this._triggerEvent("complete");
            }
            else
                return this.options.value;
        },
        _setOption: function (key, value) {
            if (key == "value") {
                if (this.options.value != value) {
                    return this.val(value);
                }
            }
            return $.Widget.prototype._setOption.call(this, key, value);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });

    $.widget("dna.taoUploadInfo", {
        options: {
            uploader: null,
            uploadText: "Upload",
            layout: null //possible values null | inline
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();

            ///Build info UI
            var tb = $("<table/>").appendTo(el)
                                              .css({
                                                  "width": "100%",
                                                  "margin": "5px"
                                              }),
             row = $("<tr/>").appendTo(tb),

            countLabel = $("<td/>").text(opts.uploadText + " 0/0"),
            progressLabel = $("<td/>").text("0%"),
            progressCell = $("<td/>"),
            progressbar = $("<div/>").appendTo(progressCell).taoProgressbar(),
           uploader = $.type(opts.uploader) == "string" ? $(opts.uploader) : opts.uploader;

            if (opts.layout == "inline") {
                row.append(countLabel.css({ "white-space": "nowrap" }))
                      .append(progressCell.css({ width: "80%" }))
                      .append(progressLabel);
            }
            else {
                countLabel.css({ "width": "80%", "text-indent": "5px", "text-align": "left" });
                progressLabel.css({ width: "20%", "text-align": "right", "padding-right": "5px" });
                row.append(countLabel).append(progressLabel);
                progressCell.attr("colspan", 2);
                $("<tr/>").append(progressCell).appendTo(tb);
            }

            this.wrapper = tb;
            this.countLabel = countLabel;
            this.progressLabel = progressLabel;
            this.progressbar = progressbar;
            var count = 0;

            if (uploader) {
                if (uploader.length) {
                    var prefix = "taoUploader";
                    uploader.bind(prefix + "start", function (evt, dat) {
                        count = dat.count;
                    })
                    uploader.bind(prefix + "progress", function (evt, dat) {
                        self.setInfo({ index: dat.index, count: count, progress: dat.progress });
                    });
                }
            }
        },
        setInfo: function (data) {
            ///<summary>Set the info object to this widget</summary>
            ///<remark>the data object is {index:[fileIndex],count:[filesCount],progress:[Current upload progress] }</remark>
            var opts = this.options;
            if (data.count != undefined && data.index != undefined)
                this.countLabel.text(opts.uploadText + " " + (data.index + 1) + "/" + data.count);

            if (data.progress != undefined) {
                this.progressLabel.text(data.progress + "%");
                this.progressbar.taoProgressbar("val", data.progress);
            }
        },
        _unobtrusive: function () {
            var opts = this.options, el = this.element;
            if (el.data("uploader"))
                opts.uploader = el.datajQuery("uploader");
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
        widget: function () {
            return this.wrapper;
        },
        destroy: function () {
           // this.wrapper.after(this.element);
           // this.wrapper.remove();
            $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);