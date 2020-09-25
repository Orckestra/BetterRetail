/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    'use strict';

    export const enum PasswordCheckStrength {
        Short = 'short',
        Common = 'common',
        Weak = 'weak',
        Ok = 'ok',
        Strong = 'strong',
    }

    export class PasswordCheckService {
        private readonly minimumLength: number;
        private readonly passwordPattern: RegExp;

        constructor({minimumLength = 6, passwordPattern}) {
            this.minimumLength = minimumLength;
            this.passwordPattern = passwordPattern;
        }

        public isPasswordCommon(password: string): boolean {
            return this.passwordPattern.test(password);
        }

        //
        // Returns the strength of the current password
        //
        public checkPasswordStrength(password: string): PasswordCheckStrength {
            let numberOfElements = 0;
            /.*[a-z].*/.test(password) ? ++numberOfElements : numberOfElements;      // Lowercase letters
            /.*[A-Z].*/.test(password) ? ++numberOfElements : numberOfElements;      // Uppercase letters
            /.*[0-9].*/.test(password) ? ++numberOfElements : numberOfElements;      // Numbers
            /[^a-zA-Z0-9]/.test(password) ? ++numberOfElements : numberOfElements;   // Special characters (inc. space)

            // Check then strength of this password using some simple rules
            if (password === null || password.length < this.minimumLength) {
                return PasswordCheckStrength.Short;
            }
            if (!this.isPasswordCommon(password)) {
                return PasswordCheckStrength.Common;
            }
            if (numberOfElements <= 2) {
                return PasswordCheckStrength.Weak;
            }
            if (numberOfElements === 3) {
                return PasswordCheckStrength.Ok;
            }

            return PasswordCheckStrength.Strong;
        }
    }
}
