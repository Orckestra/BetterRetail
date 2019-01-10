/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./IPlugin.ts' />

module Orckestra.Composer {
    export class ParsleyAccessibilityPlugin implements IPlugin {

        public initialize() {

            (<IParsleyValidator>window)['Parsley'].on('field:error', function () {

                this.$element.attr('aria-invalid', 'true');
                if (this.$element.attr('data-content')) {
                    this.$element.attr('data-errorId', this._ui.errorsWrapperId);
                } else {
                    this.$element.attr('data-errorId', this._ui.errorsWrapperId);
                    this.$element.attr('aria-describedby', this._ui.errorsWrapperId);
                }

                this._ui.$errorsWrapper.attr('aria-live', 'polite');
            });

            (<IParsleyValidator>window)['Parsley'].on('field:success', function () {
                if (this.$element.attr('aria-invalid')) {
                    this.$element.removeAttr('aria-invalid');
                }
            });

            $('input[data-content]').on('shown.bs.popover', (e: Event) => {
                let element$ = $(e.target);

                if (element$.attr('aria-invalid') === 'true') {
                    element$.attr('aria-describedby', element$.attr('data-errorId') + ' ' + element$.attr('aria-describedby'));
                }
            }).on('hidden.bs.popover', (e: any) => {
                let element$ = $(e.target);

                if (element$.attr('aria-invalid') === 'true') {
                    element$.attr('aria-describedby', element$.attr('data-errorId'));
                }
            });
        }
    }
}
