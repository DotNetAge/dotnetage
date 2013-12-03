/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoPager", {
        options: {
            totalpages: 50,
            totalrecords: 0,
            pageindex: 1,
            pagesize: 20,
            pagerClass: null,
            datasource: null,
            changed: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._unobtrusive();

            if (!el.hasClass("d-pager"))
                el.addClass("d-pager");

            if (opts.changed)
                el.bind(eventPrefix + "changed", opts.changed);

            if (opts.datasource) {
                opts.datasource.bind("taoDataSourcechanged", function () {
                    opts.totalpages = opts.datasource.taoDataSource("totalPages");
                    opts.totalrecords = opts.datasource.taoDataSource("total");
                    opts.pagesize = opts.datasource.taoDataSource("option", "pageSize");
                    opts.pageindex = opts.datasource.taoDataSource("option", "pageIndex");
                    el.empty();
                    self._initUI();
                })
            } else
                self._initUI();
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("source")) opts.datasource = el.datajQuery("source");
            if (el.data("onbind")) opts.dataBind = new Function("event", "ui", el.data("onbind"));
            if (el.data("change")) opts.changed = new Function("event", "ui", el.data("change"));
            if (el.data("tmpl")) opts.tmpl = el.datajQuery("tmpl");
            if (el.data("pages") != undefined) opts.totalpages = el.dataInt("pages");
            if (el.data("size") != undefined) opts.pagesize = el.dataInt("size");
            if (el.data("index") != undefined) opts.pageindex = el.dataInt("index");
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
        },
        _initUI: function () {
            var self = this, opts = this.options, el = this.element;

            if (el.children().length) {
                el.children().each(function (i, n) {
                    $(n).attr("data-index", i);
                });
            } else {
                for (var i = 1; i <= opts.totalpages; i++) {
                    el.append($("<a/>").attr("data-index", i)
                                                     .addClass("d-ui-widget")
                                                     .addClass("d-button")
                                                     .attr("href", "javascript:void(0);")
                                                     .text(i));
                }
            }

            $("a", el).on("click", function () { self.go(parseInt($(this).data("index"))); })
                           .on("mouseover", function () { $(this).isHover(true); })
                           .on("mouseleave", function () { $(this).isHover(false); })

            var _cur = $("[data-index='" + opts.pageindex + "']", el);
            if (_cur.length)
                _cur.isActive(true);
        },
        refresh: function () {
            this.options.pageindex = 1;
            this.element.empty();
            this._initUI();
            console.log(this.options.totalpages);
        },
        go: function (index) {
            var self = this, opts = this.options, el = this.element,
                 _selected = $(".d-state-active", el), _cur = $("[data-index='" + index + "']", el);

            if (_selected.length) _selected.isActive(false);

            if (_cur.length) _cur.isActive(true);

            //console.log(opts.pageindex);

            if (opts.pageindex != index) {
                opts.pageindex = index;
                self._triggerEvent("changed", index);
            }

            if (opts.datasource) {
                opts.datasource.taoDataSource("page", index);
            }
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);