/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class ProductVideoController extends Orckestra.Composer.Controller {
        protected videoPlayer;
        protected videoModalElement: HTMLElement;
        protected currentVideoUrl: string;

        public initialize() {
            super.initialize();
            this.initThumbnails();
            this.initModal();
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

        protected initThumbnails(): void {
            document.querySelectorAll('a.video-thumbnail').forEach((el: Element) => {
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
    }
}