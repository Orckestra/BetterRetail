///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />
///<reference path='../Mvc/ComposerClient.ts'/>

module Orckestra.Composer {
  export class LazyController extends Controller {
    public initialize() {
      super.initialize();
      this.loadContent();
    }

    public loadContent() {
      let request = this.context.container.data('request');
      if (request) {
        //TODO: Add loader
        ComposerClient.post('/api/partial/render', request).then((payload) => {
          this.replaceContent(payload);
        });
      }
    }

    private replaceContent(newContent: string) {
      var controllerRegistry: Orckestra.Composer.ControllerRegistry = new Orckestra.Composer.ControllerRegistry(),
        controller: Orckestra.Composer.IController;

      let newHtml = $(newContent);
      this.context.container.replaceWith(newHtml);

      var blades = newHtml.find('[data-oc-controller]').addBack('[data-oc-controller]'),
        controllers: IController[] = [];

      blades.each((index: number, item: HTMLElement) => {

        var bladeName: string = item.getAttribute('data-oc-controller'),
          context: Orckestra.Composer.IControllerContext;

        if (controllerRegistry.isRegistered(bladeName)) {
          context = {
            container: $(item),
            dataItemId: item.getAttribute('data-item-id'),
            templateName: bladeName,
            viewModel: JSON.parse(item.getAttribute('data-context') || window[item.getAttribute('data-context-var')] || '{}'),
            window: window
          };

          controller = Orckestra.Composer.ControllerFactory.createController({
            controllerName: bladeName,
            context: context,
            eventHub: this.eventHub,
            composerContext: this.composerContext,
            composerConfiguration: this.composerConfiguration
          });

          controller.initialize();
          controllers.push(controller);
        }
      });

      $(window).on('beforeunload', () => {
        controllers.forEach(controller => controller.dispose());
      });
    }
  }
}