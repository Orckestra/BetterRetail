VersionInputSelectorBinding.prototype = new DataInputSelectorBinding;
VersionInputSelectorBinding.prototype.constructor = VersionInputSelectorBinding;
VersionInputSelectorBinding.superclass = DataInputSelectorBinding.prototype;

/**
* @class
*/
function VersionInputSelectorBinding() {

	/**
	* @type {SystemLogger}
	*/
	this.logger = SystemLogger.getLogger("VersionInputSelectorBinding");

}

/**
* Identifies binding.
*/
VersionInputSelectorBinding.prototype.toString = function () {

	return "[VersionInputSelectorBinding]";
};


/**
* @overloads {DataInputSelectorBinding#onBindingAttach}
*/
VersionInputSelectorBinding.prototype.onBindingAttach = function () {

	VersionInputSelectorBinding.superclass.onBindingAttach.call(this);

	DOMEvents.addEventListener(this.shadowTree.input, DOMEvents.KEYUP, this);

	this.isRequired = true;
};

/**
 * @implements {IEventListener}
 * @overloads {Binding#handleEvent}
 * @param {Event} e
 */
VersionInputSelectorBinding.prototype.handleEvent = function (e) {

	VersionInputSelectorBinding.superclass.handleEvent.call(this, e);

	if (this.isFocusable == true) {
		switch (e.type) {
			case DOMEvents.KEYUP:
				if (this._selectedItemBinding && this._selectedItemBinding.selectionValue != this.getValue()) {
					this._selectedItemBinding = null;
					var field = this.getAncestorBindingByLocalName ( "field" );
					if (field && field instanceof FieldBinding) {
						var fieldid = field.getProperty("id");
						var fieldrelelements = field.bindingDocument.querySelectorAll("[fieldrel='" + fieldid + "']");
						new List(fieldrelelements).each(function (fieldrelelement) {
							var fieldrel = UserInterface.getBinding(fieldrelelement);
							if (fieldrel && field instanceof FieldBinding) {
								fieldrel.getDescendantBindingsByType(DataInputBinding).each(function (datainputbinding) {
									datainputbinding.setReadOnly(false);
								}, this);
							}
						},this);
					}
				}

				break;
		}
	}
}

/**
* @param {boolean} isReadOnly
* @overloads {DataInputBinding#setReadOnly}
*/
VersionInputSelectorBinding.prototype.setReadOnly = function (isReadOnly) {

    //nothing
}