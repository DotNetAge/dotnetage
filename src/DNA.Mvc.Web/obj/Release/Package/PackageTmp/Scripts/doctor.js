$.seo = new function () {
    this.analyse = function (url) {
        var dfd = new $.Deferred(), _start = new Date();
        $.ajax(url)
          .done(function (data) {
              var report = {
                  has_title: false,
                  has_keywords: false,
                  has_desc: false,
                  title_len: 0,
                  desc_len: 0,
                  keywords_len: 0,
                  links: 0,
                  no_alt_imgs: 0,
                  h1_count: 0,
                  h2_count: 0,
                  loading_time: (new Date().getTime()) - _start.getTime()
              },
                  _helper = $("<div/>").appendTo("body"),
                  _frameHtml = "<iframe src='javascript:void(0);' style='display:none;'></iframe>";

              _helper.append(_frameHtml);
              var _frame = $("iframe", _helper)[0];
              var _contentWin = _frame.contentWindow;
              _contentWin.document.designMode = 'on';
              _contentWin.document.open();
              _contentWin.document.write(data);
              _contentWin.document.close();
              var _head = $("head", _contentWin.document);
              //title
              var _title = _head.children("title");
              report.has_title = _title.length > 0;
              if (_title.length)
                  report.title_len = _title.text().length;

              //desc
              var _desc = $("meta[name='description']", _head);
              report.has_desc = _desc.length > 0;
              if (_desc.length)
                  report.desc_len = _desc.text().length;

              //keywords
              var _keys = $("meta[name='keywords']", _head);
              report.has_keywords = _keys.length > 0;
              if (_keys.length)
                  report.keywords_len = _keys.text().length;

              //number of links
              report.links = $("body a[rel!='nofollow']", _contentWin.document).length;

              //page rank flow

              //page indexable

              //alt image tag
              var _imgs = $("body img", _contentWin.document);
              report.no_alt_imgs = _imgs.length - $("[alt]", _imgs).length;

              //h1
              report.h1_count = $("body h1", _contentWin.document).length;

              //h2
              report.h2_count = $("body h2", _contentWin.document).length;
              _helper.remove();
              //console.log(report);
              dfd.resolve(report);
          })
          .fail(function () { dfd.reject(); });
        return dfd;
    }
}