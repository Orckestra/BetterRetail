///<reference path='../Product/ProductController.ts' />

module Orckestra.Composer {
    export class ProductZoomController extends Orckestra.Composer.ProductController {

        private allImages = {};

        public initialize() {

            super.initialize();
            this.initZoom();

            this.eventHub.subscribe('productDetailSelectedVariantIdChanged', (e) => this.updateModalImages(e));
        }

        protected openZoom(event: JQueryEventObject) {
            event.preventDefault();
            $('.modal-fullscreen').modal();
        }

        protected changeZoomedImage(event: JQueryEventObject) {
            let context$: JQuery = $(event.target),
                largeImage: HTMLImageElement = <HTMLImageElement>document.querySelector('.js-zoom-image'),
                selector: string = event.target.tagName, // Clicked HTML element
                $largeImage: JQuery = $(largeImage);
            event.preventDefault();

            if (selector.toLocaleLowerCase() === 'img') {
                var src = context$.attr('data-zoom-src');
                $('.js-zoom-thumbnails').find('a').removeClass('active');
                context$.parent().addClass('active');
                $largeImage.attr('src', src);
            }
        }

        protected errorZoomedImage(event: JQueryEventObject) {
            var $element = $(event.target),
                fallbackImageUrl = $element.attr('data-fallback-image-url');

            $element.attr('src', fallbackImageUrl);
        }

        protected initZoom() {
            $(document).on('click', '.js-zoom', (event: JQueryEventObject) => this.openZoom(event));
            $(document).on('click', '.js-zoom-thumbnails', (event: JQueryEventObject) => this.changeZoomedImage(event));
            $('.js-zoom-image').on('error', (event: JQueryEventObject) => this.errorZoomedImage(event));
        }

        protected updateModalImages(e) {
            const zoomThumbnailsContainer: HTMLElement = document.querySelector('.js-zoom-thumbnails');
            let selectedVariantThumbnails: NodeListOf<Element>;
            //For regular "Images.cshtml" view, available for Grocery and Retail
            const productThumbnails: HTMLElement = document.querySelector('.product-thumbnails');
            //For "ImagesLargeGallery.cshtml" and "ThumbsGallery.cshtml" views combo, only available on Retail (and current default)
            const mobileCarouselThumbnails: HTMLElement = document.querySelector('.mobile-carousel-thumbnails');

            if (productThumbnails) {
                selectedVariantThumbnails = document.querySelectorAll('.product-thumbnails .js-thumbnails[data-variant="' + e.data.selectedVariantId + '"]');
            } else if (mobileCarouselThumbnails) {
                selectedVariantThumbnails = document.querySelectorAll('.mobile-carousel-thumbnails a.thumbnail[data-variant="' + e.data.selectedVariantId + '"]');
            }

            //Clear the zoom thumbnails
            zoomThumbnailsContainer.innerHTML = '';
            //Add the selected variant thumbs to the zoom modal box by cloning/appending them
            selectedVariantThumbnails.forEach((el: HTMLElement, index: number) => {
                let clonedThumbnail: HTMLElement = <HTMLElement>el.cloneNode(true);

                //Make sure to remove the 'selectImage' event, it's not needed in the zoom
                clonedThumbnail.removeAttribute('data-oc-click');
                zoomThumbnailsContainer.appendChild(clonedThumbnail);
            });
            //Once all thumbnails are copied, update the main zoom image
            (<HTMLElement>zoomThumbnailsContainer.querySelector('.js-thumbnails.active img')).click();
        }
    }
}
