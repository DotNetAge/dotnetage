$(function () {
    var _uri = widget.preferences.getItem("uri");
    $("body").addClass("loading");
    var _body = widget.getBodyElement();

    $.ajax("/syndication/read?uri=" + encodeURI(_uri))
      .done(function (data) {
          var rows = widget.preferences.getItem("maxrows"), results = data.Items;
          if (rows > 0) {
              results = [];
              $.each(data.Items, function (i, n) {
                  if (i < rows) results.push(n); else return;
              });
          }
          $("#article_tmpl").tmpl(results).appendTo($("#feedcontainer"));
          
          if (widget.preferences.getItem("autoheight"))
              $(_body).height($(document).height());

          if (data.Title) {
              if (data.Title.Text)
                  widget.setTitle(data.Title.Text);
              if (data.Links)
                  widget.setLink(data.Links.pop().Uri);
              if (data.ImageUrl)
                  widget.setIcon(data.ImageUrl);
          }
          //console.log(widget);
          $("body").removeClass("loading");
      }).fail(function () {
          $("body").removeClass("loading");
          $("<div>There is an error occour when loading feed.</div>").appendTo(_body);
      });
});