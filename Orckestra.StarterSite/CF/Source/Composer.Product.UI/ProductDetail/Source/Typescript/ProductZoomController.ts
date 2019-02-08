///<reference path='../../../Product/Source/Typescript/ProductController.ts' />

module Orckestra.Composer {
    export class ProductZoomController extends Orckestra.Composer.ProductController {

        public initialize() {

            super.initialize();
            this.initZoom();
        }

        protected openZoom(event: JQueryEventObject) {
            let index: number = parseInt($('.js-thumbnail.active').attr('data-index'), 10);

            $('.js-zoom-thumbnail').eq(index).click();
            event.preventDefault();
            $('.modal-fullscreen').modal();
        }

        protected changeZoomedImage(event: JQueryEventObject) {
            let context$ = $(event.target),
                largeImage: HTMLImageElement = <HTMLImageElement>document.querySelector('.js-zoom-image'),
                selector: string = event.target.tagName, // Clicked HTML element
                $largeImage: JQuery = $(largeImage);

            if (selector.toLocaleLowerCase() !== 'a') {
                context$ = context$.parent();
            }

            $('.js-zoom-thumbnail').removeClass('active');

            context$.addClass('active');
            $largeImage.attr('src', context$.attr('data-image'));
        }

        protected errorZoomedImage(event: JQueryEventObject) {
            let $element = $(event.target),
                fallbackImageUrl = $element.attr('data-fallback-image-url');

            $element.attr('src', fallbackImageUrl);
        }

        protected initZoom() {
            $(document).on('click', '.js-zoom', (event: JQueryEventObject) => this.openZoom(event));
            $(document).on('click', '.js-zoom-thumbnail', (event: JQueryEventObject) => this.changeZoomedImage(event));
            $('.js-zoom-image').on('error', (event: JQueryEventObject) => this.errorZoomedImage(event));

            // Select first thumbnail
            $('.js-zoom-thumbnail').eq(0).click();
        }
    }
}
