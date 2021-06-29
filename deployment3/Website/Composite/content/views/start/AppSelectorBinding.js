AppSelectorBinding.prototype = new PageBinding;
AppSelectorBinding.prototype.constructor = AppSelectorBinding;
AppSelectorBinding.superclass = PageBinding.prototype;

AppSelectorBinding.VIEW_CLASSNAME = "app-selector-view";
/**
 * @class
 */
function AppSelectorBinding() {

    /**
	 * @type {SystemLogger}
	 */
    this.logger = SystemLogger.getLogger("AppSelectorBinding");

    /**
	 * True when Start screen is visible.
	 * @type {boolean}
	 */
    this._isShowingStart = false;

    /**
	 *  @type {ViewBinding}
	 */
    this._view = null;

}

/**
 * Identifies binding.
 */
AppSelectorBinding.prototype.toString = function () {

    return "[AppSelectorBinding]";
}

/**
 * @overloads {PageBinding#onBindingRegister}
 */
AppSelectorBinding.prototype.onBindingRegister = function () {

    AppSelectorBinding.superclass.onBindingRegister.call(this);
    EventBroadcaster.subscribe(BroadcastMessages.START_COMPOSITE, this);
    EventBroadcaster.subscribe(BroadcastMessages.STOP_COMPOSITE, this);
    EventBroadcaster.subscribe(BroadcastMessages.KEY_ESCAPE, this);
    EventBroadcaster.subscribe(BroadcastMessages.STAGE_INITIALIZED, this);

    this._view = this.getAncestorBindingByType(ViewBinding, true);
    if (this._view) {
        DOMEvents.addEventListener(this._view.bindingElement, DOMEvents.CLICK, this);
        this._view.attachClassName(AppSelectorBinding.VIEW_CLASSNAME);
    }
}

/**
 * Load start page with a random querystring to avoid client cache.
 * @overloads {PageBinding#onBindingAttach}
 */
AppSelectorBinding.prototype.onBindingAttach = function () {
    var self = this;
    AppSelectorBinding.superclass.onBindingAttach.call(self);

    function onHtmlload() {
        var request = this;
        if (request.responseText) {
            document.getElementById("applications").innerHTML = request.responseText;
            var currentApp = document.getElementsByClassName('current');
            if (currentApp && currentApp.length > 0) {
                currentApp[0].onclick = function (e) {
                    self.stop();
                    e.preventDefault();
                }
            };
        }
    }

    try {
        var request = new XMLHttpRequest();
        request.onload = onHtmlload;
        request.open('GET', "/experiencemanagement/applications?random=" + KeyMaster.getUniqueKey());
        request.send();
    } catch (ex) { }

}

/**
 * @overloads {PageBinding#handleEvent}
 * @implements {IEventListener}
 * @param {Event} e
 */
AppSelectorBinding.prototype.handleEvent = function (e) {

    AppSelectorBinding.superclass.handleEvent.call(this, e);
    var element = DOMEvents.getTarget(e);
    switch (e.type) {
        case DOMEvents.CLICK:
            if (this._view && this._view.bindingElement == element) {
                this.stop();
            }
            break;
    }
}

/**
 * Open app selector page
 */
AppSelectorBinding.prototype.start = function () {

    EventBroadcaster.subscribe(top.WindowManager.WINDOW_RESIZED_BROADCAST, this);
    var self = this;
    self._view.show();
    setTimeout(function () {
         self._view.attachClassName("active");
    }, 20);

    this.updateLayout();
}

/**
 * Close app selector page
 */
AppSelectorBinding.prototype.stop = function () {

    EventBroadcaster.unsubscribe(top.WindowManager.WINDOW_RESIZED_BROADCAST, this);
    var self = this;
    self._view.detachClassName("active");
    setTimeout(function () { self._view.hide(); }, 500);
}


/**
 * @implements {IBroadcastListener}
 * @overloads {PageBinding#handleBroadcast}
 * @param {string} broadcast
 * @param {object} arg
 */
AppSelectorBinding.prototype.handleBroadcast = function (broadcast, arg) {

    AppSelectorBinding.superclass.handleBroadcast.call(this, broadcast, arg);

    switch (broadcast) {

        case BroadcastMessages.START_COMPOSITE:
            this._isShowingStart = true;
            this.start();
            if (top.app.bindingMap.explorermenu) {
            	top.app.bindingMap.explorermenu.collapse();
            }
            break;

        case BroadcastMessages.STOP_COMPOSITE:
            this.stop();
            break;
        case BroadcastMessages.KEY_ESCAPE:
            if (this._isShowingStart) {
                this.stop();
            }
            break;
        case BroadcastMessages.STAGE_INITIALIZED:
            if (ViewBinding.hasInstance("Composite.Management.Start")) {
                ViewBinding.getInstance("Composite.Management.Start").hide();
            }
            break;
        case top.WindowManager.WINDOW_RESIZED_BROADCAST:
            this.updateLayout();
            break;
    }
}


/**
 * Update width and height of app selector
 */
AppSelectorBinding.prototype.updateLayout = function () {

    var windowElement = this._view.windowBinding.bindingElement;
    var applicationsWrapper = this.bindingDocument.querySelector(".applications-wrapper");
    windowElement.style.removeProperty("max-width");
    windowElement.style.removeProperty("height");
    if (applicationsWrapper && applicationsWrapper.offsetWidth) {
        windowElement.style.maxWidth = applicationsWrapper.offsetWidth + "px";
        windowElement.style.setProperty("height", applicationsWrapper.offsetHeight + "px", "important");
    }
}