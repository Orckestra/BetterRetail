VersionPageBinding.prototype = new PageBinding;
VersionPageBinding.prototype.constructor = VersionPageBinding;
VersionPageBinding.superclass = PageBinding.prototype;


VersionPageBinding.UPDATE_MESSAGE = "update page version shedule view";

/**
 * @class
 * VersionPageBinding.
 * @param {DOMElement} bindingElement
 */
function VersionPageBinding() {

	/**
	 * @type {SystemLogger}
	 */
	this.logger = SystemLogger.getLogger("VersionPageBinding");

	/*
	 * Return this.
	 */
	return this;
}

/**
 * Identifies binding.
 */
VersionPageBinding.prototype.toString = function () {

	return "[VersionPageBinding]";
}

/**
 * @overloads {Binding#onBindingRegister}
 */
VersionPageBinding.prototype.onBindingRegister = function () {

	VersionPageBinding.superclass.onBindingRegister.call(this);
}

/**
 * Overloads {Binding#onBindingAttach}
 */
VersionPageBinding.prototype.onBindingAttach = function () {

	VersionPageBinding.superclass.onBindingAttach.call(this);

	DOMEvents.addEventListener(this.bindingDocument.documentElement, UpdateManager.EVENT_AFTERUPDATE, this);

	this.subscribe(VersionPageBinding.UPDATE_MESSAGE);


	if (!this.shadowTree.inputconsoleid) {
		var input = DOMUtil.createElementNS(
			Constants.NS_XHTML, "input", this.bindingDocument
		);
		input.type = "hidden";
		input.name = "__CONSOLEID";
		input.value = Application.CONSOLE_ID;
		this.shadowTree.inputconsoleid = input;
		this.bindingElement.appendChild(input);
	}
}

/**
 * @implements {IEventListener}
 * @overloads {Binding#handleEvent}
 * @param {MouseEvent} e
 */
VersionPageBinding.prototype.handleEvent = function (e) {

	VersionPageBinding.superclass.handleEvent.call(this, e);

	var target = e.currentTarget ? e.currentTarget : DOMEvents.getTarget(e);

	switch (e.type) {

		case UpdateManager.EVENT_AFTERUPDATE:
			if (target === this.bindingDocument.documentElement) {
				MessageQueue.update();
			}
			break;
	}
}

/**
 * Implements {IBroadcastListener}
 * @param {string} broadcast
 * @param {object} arg
 */
VersionPageBinding.prototype.handleBroadcast = function (broadcast, arg) {

	VersionPageBinding.superclass.handleBroadcast.call(this, broadcast, arg);

	switch (broadcast) {

		case VersionPageBinding.UPDATE_MESSAGE:
			if (!arg || this.getProperty("page-id") == arg) {
				if (this._canPostBack) {
					var callbackid = "UPDATE";
					var callbackarg = "";
					this.bindingWindow.__doPostBack(callbackid, callbackarg);
				}
			}
			break;
	}
}