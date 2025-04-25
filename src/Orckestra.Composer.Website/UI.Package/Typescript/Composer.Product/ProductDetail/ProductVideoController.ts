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

        protected loadVideo(videoUrl: string, videoTitle: string): void {
            const videoElement: HTMLVideoElement = <HTMLVideoElement>this.videoModalElement.querySelector('#product-video');

            if ((this.videoPlayer && this.currentVideoUrl !== videoUrl) || !this.videoPlayer) {
                this.videoModalElement.querySelector('#video-modal-label').textContent = videoTitle;

                if (!this.videoPlayer) {
                    videoElement.src = videoUrl;
                    // @ts-ignore
                    this.videoPlayer = videojs('product-video', {fluid: true, controls: true, preload: 'auto'});
                } else {
                    this.videoPlayer.src(videoUrl);
                }

                this.currentVideoUrl = videoUrl;
            }

            $(this.videoModalElement).modal('show');
        }

        protected initCtas(): void {
            document.querySelectorAll('a.pdp-video-cta').forEach((el: Element) => {
                el.addEventListener('click', (e: Event) => {
                    e.preventDefault();

                    const el: HTMLElement = <HTMLElement>e.currentTarget;
                    this.loadVideo(el.dataset.videoUrl, el.getAttribute('title'));
                })
            })
        }

        protected initModal(): void {
            this.videoModalElement = document.querySelector('#video-modal');

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