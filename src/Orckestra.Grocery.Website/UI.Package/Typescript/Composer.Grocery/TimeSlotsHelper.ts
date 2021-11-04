/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
	'use strict';

	export class TimeSlotsHelper {

		public static getTimeSlotReservationExpireDayIndex(timeSlotReservation: any) {
			if (timeSlotReservation == null) return -1;

			let reservationExpiration = moment(timeSlotReservation.ExpiryDateTime);

			let now = moment();
			let today = moment().endOf('day');
			let tomorrow = moment().add(1, 'day').endOf('day');

			if (reservationExpiration < now) return -1;
			if (reservationExpiration < today) return 0;
			if (reservationExpiration < tomorrow) return 1;

			return -1;
		}

		public static getTimeSlotReservationExpireTime(timeSlotReservation: any) {
			return timeSlotReservation && moment(timeSlotReservation.ExpiryDateTime).format('LT');
		}

		public static getTimeSlotReservationExpireDate(timeSlotReservation: any) {
			return timeSlotReservation && moment(timeSlotReservation.ExpiryDateTime).format('d-MM-yyyy');
		}

		public static isTimeSlotReservationExpired(timeSlotReservation: any) {
			return timeSlotReservation && timeSlotReservation.ReservationStatus == 3;
		}

		public static isTimeSlotReservationTentative(timeSlotReservation: any) {
			return timeSlotReservation && timeSlotReservation.ReservationStatus == 1;
		}

		public static validateTimeSlotExpiration(timeSlotReservation): boolean {
			let slotTime = new Date(Date.parse(timeSlotReservation.ExpiryDateTime));
			let now = new Date();
			return !(slotTime < now)
		}

		public static getCommonTimeSlotReservationVueConfig(): any {
			return {
				computed: {
					TimeSlotReservationExpireTime() {
						return this.SelectedStore && TimeSlotsHelper.getTimeSlotReservationExpireTime(this.SelectedStore.TimeSlotReservation);
					},
					TimeSlotReservationExpireDate() {
						return this.SelectedStore && TimeSlotsHelper.getTimeSlotReservationExpireDate(this.SelectedStore.TimeSlotReservation);
					},
					TimeSlotReservationExpireRelativeDayIndex() {
						return this.SelectedStore && TimeSlotsHelper.getTimeSlotReservationExpireDayIndex(this.SelectedStore.TimeSlotReservation);
					},
					TimeSlotReservationExpired() {
						return this.SelectedStore && TimeSlotsHelper.isTimeSlotReservationExpired(this.SelectedStore.TimeSlotReservation);
					},
					TimeSlotReservationTentative() {
						return this.SelectedStore && TimeSlotsHelper.isTimeSlotReservationTentative(this.SelectedStore.TimeSlotReservation);
					}
				}
			}
		};

		public static getTimeSlotReservationError(errorCode) {
			let error = LocalizationProvider.instance().getLocalizedString('Grocery', 'L_' + errorCode);
			if (!error) {
				error = LocalizationProvider.instance().getLocalizedString('Grocery', 'L_TimeSlotReservationFailed');
			}
			return error;
		}

		public static getTimeSlotLocalizations(timeslotDate: any, start: any, end: any, culture: string) {
			let lang = culture.substring(0, 2);

			let parsedStartTime = new Date(moment(start, 'HH:mm:ss').format());
			let parsedEndTime = new Date(moment(end, 'HH:mm:ss').format());
			let timeOptions = { hour: "numeric", minute: "2-digit" };
			let localizedStartTime = new Intl.DateTimeFormat(lang, timeOptions).format(parsedStartTime);
			let localizedEndTime = new Intl.DateTimeFormat(lang, timeOptions).format(parsedEndTime);

			var date = new Date(timeslotDate);
			let dateOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
			let localizedDate = date.toLocaleDateString(lang, dateOptions);

			return {
				LocalizedSlotBeginTime: localizedStartTime,
				LocalizedSlotEndTime: localizedEndTime,
				LocalizedSlotDate: localizedDate
			};
		}
	}
}
