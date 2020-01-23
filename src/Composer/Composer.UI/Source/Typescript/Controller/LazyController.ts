///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />
///<reference path='../Mvc/ComposerClient.ts'/>

module Orckestra.Composer {
  export class LazyController extends Controller {
    public initialize() {
      super.initialize();
      this.loadContent();
      console.log('LazyController initialize');
    }

    public loadContent() {
      let request = this.context.container.data('request');
      let container = this.context.container;
      if (request) {
        $.ajax({
          url: '/api/partial/render',
          type: 'POST',
          data: JSON.stringify(request),
          contentType: 'application/json; charset=utf-8',
          success: function (a) {
            //container.replaceWith(a);
            $('[replace-this]').replaceWith(a);
          }
        });
        // ComposerClient.post('/api/partial/render', request).then((payload) => {
        //   console.log(payload);
        //   //container.replaceWith('<div>!123234</div>');
        //   container.replaceWith(payload);
        //   $('[replace-this]').replaceWith(payload);

        // });
      }
    }

    // private sendRequest(data?: any) {
    //   var settings: EnhancedJQueryAjaxSettings = {
    //     contentType: 'application/json',
    //     dataType: 'json',
    //     data: data ? JSON.stringify(data) : null,
    //     method: "POST",
    //     url: url,
    //     headers: {
    //         'Accept-Language': this.getPageCulture(),
    //         'WebsiteId': this.getWebsiteId()
    //     }
    // };

    //   return Q($.ajax(settings)).fail(reason => this.onRequestRejected(reason));
    // }
  }
}