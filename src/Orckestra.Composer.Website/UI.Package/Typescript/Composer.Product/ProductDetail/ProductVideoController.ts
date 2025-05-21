/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class ProductVideoController extends Orckestra.Composer.Controller {
        readonly youtubeUrlPrefix: string = 'https://www.youtube.com/embed/';
        protected videoPlayer;
        protected videoModalElement: HTMLElement;
        protected videoJsContainer: HTMLElement = this.context.container[0].querySelector('.vjs-16-9');
        protected videoJsElement: HTMLVideoElement = this.videoJsContainer.querySelector('.video-js');
        protected youtubeElement: HTMLIFrameElement = this.context.container[0].querySelector('iframe');
        protected currentVideoUrl: string;
        protected isLoadedVideoFromYoutube: boolean = false;

        public initialize() {
            super.initialize();
            this.initCtas();
            this.initModal();
            this.initVideosSection();
        }

        /**
         * Displays the VideoJs player and hide the YouTube iframe
         * @protected
         */
        protected showVideoJsPlayer() {
            this.videoJsContainer.classList.remove('d-none');
            this.youtubeElement.parentElement.classList.add('d-none');
        }

        /**
         * Displays the YouTube iframe and hide the VideoJs player
         * @protected
         */
        protected showYoutubeIframe() {
            this.youtubeElement.parentElement.classList.remove('d-none');
            this.videoJsContainer.classList.add('d-none');
        }

        /**
         * Extracts the YouTube video ID from most YouTube urls
         * @param url - YouTube video url
         * @protected
         */
        protected getYoutubeId(url: string): string {
            let id: string;
            let splitUrl: string[];

            splitUrl = url.replace(/(>|<)/gi, '').split(/(vi\/|v=|\/v\/|youtu\.be\/|\/embed\/)/);
            if (splitUrl[2] !== undefined) {
                id = (splitUrl[2].split(/[^0-9a-z_\-]/i))[0];
            } else {
                id = splitUrl[0];
            }
            return id;
        }

        /**
         * Updates the video and its title in the modal and open it.
         * @param videoUrl
         * @param videoTitle
         * @protected
         */
        protected loadVideo(videoUrl: string, videoTitle: string): void {
            //If the video URL did not change, just open the modal...
            if (this.currentVideoUrl === videoUrl) {
                $(this.videoModalElement).modal('show');
                return;
            }

            this.isLoadedVideoFromYoutube = videoUrl.indexOf('youtu') !== -1;

            //YOUTUBE VIDEO
            if (this.isLoadedVideoFromYoutube) {
                const youtubeVideoId: string = this.getYoutubeId(videoUrl); //Extract YouTube video ID

                this.youtubeElement.src = this.youtubeUrlPrefix + youtubeVideoId; //Update YouTube iframe src
                this.showYoutubeIframe(); //Toggle visibility of YouTube iframe
            //PIM VIDEO
            } else {
                //If the Video-js player is not initialized... do it!
                if (!this.videoPlayer) {
                    this.videoJsElement.src = videoUrl;
                    // @ts-ignore
                    this.videoPlayer = videojs(this.videoJsElement, {fluid: true, controls: true, preload: 'auto'});
                    this.isLoadedVideoFromYoutube = true;
                } else { //Otherwise just update the video url
                    this.videoPlayer.src(videoUrl);
                }
                this.showVideoJsPlayer(); //Toggle visibility of video-js player
            }

            //Update video title
            this.videoModalElement.querySelector('.js-video-modal-label').textContent = videoTitle;
            //Keep track of the current video, to prevent unneeded update to the player if the same video is opened multiple times
            this.currentVideoUrl = videoUrl;
            //Open modal
            $(this.videoModalElement).modal('show');
        }

        /**
         * Initializes the CTAs/thumbnails that will open the video modal.
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
         * Initializes modal box and configure its events
         * @protected
         */
        protected initModal(): void {
            this.videoModalElement = this.context.container[0].querySelector('.video-modal');

            $(this.videoModalElement).on('hide.bs.modal', () => {
                if (this.isLoadedVideoFromYoutube) {
                    //Stops YouTube video without any iframe postMessage or API call
                    this.youtubeElement.src = this.youtubeElement.src;
                } else {
                    this.videoPlayer.pause();
                    this.videoPlayer.currentTime(0);
                }
            });
        }

        /**
         * Hides the videos section if no media is visible for current variant, otherwise make it visible
         * @param videoSection - Video section container
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