(function ($) {
    $.widget("dna.taoDataSourceInfo", {
        options: {
            datasource: null,
            dataBind: null,
            tmpl: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix, _tmpl = null;

            this._unobtrusive();

            if (opts.tmpl && opts.tmpl.length)
                _tmpl = opts.tmpl;
            else
                _tmpl = $("<span>Page ${index} of ${pages} Total ${records} records found</span>");

            var _created = false;
            var srcElement = opts.datasource;

            if (srcElement && srcElement.length) {
                srcElement.bind("taoDataSourcechanged", function (e, result) {
                    var _info = {
                        records: srcElement.taoDataSource("total"),
                        pages: srcElement.taoDataSource("totalPages"),
                        index: srcElement.taoDataSource("option", "pageIndex")
                    };

                    if (_created) {
                        el.empty().append(_tmpl.tmpl(_info));
                    }
                    else {
                        el.append(_tmpl.tmpl(_info));
                        _created = true;
                    }
                    self._triggerEvent("databind", { info: _info, data: result.data });
                });
            }
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("source"))
                opts.datasource = el.datajQuery("source");
            return this;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    })

})(jQuery);
