/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class ProductVideoController extends Orckestra.Composer.Controller {
        protected videoPlayer;
        protected videoModalElement: HTMLElement;
        protected currentVideoUrl: string;

        public initialize() {
            super.initialize();
            this.initCtas();
            this.initModal();
            this.initVideosSection();
        }

        /**
         * Update the video and its title in the modal and open it.
         * @param videoUrl
         * @param videoTitle
         * @protected
         */
        protected loadVideo(videoUrl: string, videoTitle: string): void {
            const videoElement: HTMLVideoElement = <HTMLVideoElement>this.videoModalElement.querySelector('.video-js');

            //If the video url changed, or the video player is not initialized...
            if ((this.videoPlayer && this.currentVideoUrl !== videoUrl) || !this.videoPlayer) {
                //Update video title
                this.videoModalElement.querySelector('.js-video-modal-label').textContent = videoTitle;

                if (!this.videoPlayer) { //If the video player is not initialized, do it
                    videoElement.src = videoUrl;
                    // @ts-ignore
                    this.videoPlayer = videojs(videoElement, {fluid: true, controls: true, preload: 'auto'});
                } else { //Otherwise update the video url
                    this.videoPlayer.src(videoUrl);
                }

                //Keep track of the current video, to prevent unneeded update to the player if the same video is opened multiple times
                this.currentVideoUrl = videoUrl;
            }

            $(this.videoModalElement).modal('show');
        }

        /**
         * Initialize the CTAs/thumbnails that will open the video modal.
         * The 'data-ctas-container' attribute is used to target the right CTAs/thumbnails parent, in case we have the controller more than once on the page.
         * @protected
         */
        protected initCtas(): void {
            const ctasContainerSelector: string = this.context.container[0].getAttribute('data-ctas-container');

            document.querySelectorAll(ctasContainerSelector + ' a.js-pdp-video-cta').forEach((el: Element) => {
                el.addEventListener('click', (e: Event) => {
                    e.preventDefault();

                    const el: HTMLElement = <HTMLElement>e.currentTarget;
                    this.loadVideo(el.dataset.videoUrl, el.getAttribute('title'));
                })
            })
        }

        /**
         * Initialize modal box and configure its events
         * @protected
         */
        protected initModal(): void {
            this.videoModalElement = this.context.container[0].querySelector('.video-modal');

            $(this.videoModalElement).on('hide.bs.modal', () => {
                this.videoPlayer.pause();
                this.videoPlayer.currentTime(0);
            });
        }

        /**
         * Hide the videos section if no media is visible for current variant, otherwise make it visible
         * @param videoSection
         * @protected
         */
        protected switchVideosSectionVisibility(videoSection: HTMLElement): void {
            let visibleVideosList: NodeListOf<Element> = videoSection.querySelectorAll('.media:not(.d-none)');

            if (visibleVideosList.length > 0) {
                videoSection.classList.remove('d-none');
            } else {
                videoSection.classList.add('d-none');
            }
        }

        /**
         * Manage the visibility of the video section on the PDP (if the Razor function is used)
         * @protected
         */
        protected initVideosSection(): void {
            let videoSection: HTMLElement = document.getElementById('product-video-list');

            if (videoSection) {
                //On page load
                this.switchVideosSectionVisibility(videoSection);

                //On variant switch, after the media visibility has been done in the ProductDetailController
                this.eventHub.subscribe('productMediasUpdated', () => {
                    this.switchVideosSectionVisibility(videoSection)
                })

            }
        }
    }
}