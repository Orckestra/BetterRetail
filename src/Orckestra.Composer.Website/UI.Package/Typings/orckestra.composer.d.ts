/// <reference path="./tsd.d.ts" />
declare module Orckestra.Composer {
    interface IHashTable<T> {
        [key: string]: T;
    }
}

declare module Orckestra.Composer {
    interface IDisposable {
        dispose(): void;
    }
}

declare module Orckestra.Composer {
    interface IController extends IDisposable {
        initialize(): void;
        dispose(): void;
    }
}

declare module Orckestra.Composer {
    class ControllerRegistry {
        private static _instance;
        private static _registry;
        constructor();
        isRegistered(controllerName: any): boolean;
        retrieveController(controllerName: string): any;
        register(controllerName: string, controller: any): void;
        unregister(controllerName: string): any;
    }
}

declare module Orckestra.Composer {
    interface IControllerContext {
        templateName: string;
        dataItemId: string;
        container: JQuery;
        viewModel: any;
        window: Window;
    }
}

declare module Orckestra.Composer {
    interface ISubscription {
        remove(): void;
    }
}

declare module Orckestra.Composer {
    interface IListener {
        (eventInformation: IEventInformation): void;
    }
}

declare module Orckestra.Composer {
    interface IListenerQueue {
        queue: IListener[];
    }
}

declare module Orckestra.Composer {
    interface IEventInformation {
        data: any;
        source?: string;
    }
}

declare module Orckestra.Composer {
    interface IEventHub {
        subscribe(eventName: string, listener: IListener): ISubscription;
        publish(eventName: string, eventInformation: IEventInformation): void;
    }
}

declare module Orckestra.Composer {
    interface IComposerContext {
        language: string;
    }
}

declare module Orckestra.Composer {
    interface IControllerConfiguration {
        name: string;
        controller: any;
    }
}

declare module Orckestra.Composer {
    interface IComposerConfiguration {
        plugins?: string[];
        paymentProvider?: {
            name: string;
            origin: string;
            profileId: string;
        };
        controllers: IControllerConfiguration[];
    }
}

declare module Orckestra.Composer {
    interface ICreateControllerOptions {
        controllerName: string;
        context: Orckestra.Composer.IControllerContext;
        eventHub: Orckestra.Composer.IEventHub;
        composerContext: IComposerContext;
        composerConfiguration: IComposerConfiguration;
    }
}

declare module Orckestra.Composer {
    interface ICachePolicy {
        absoluteExpiration?: number;
        slidingExpiration?: number;
    }
}

declare module Orckestra.Composer {
    interface ICache {
        get<T>(key: string): Q.Promise<T>;
        set<T>(key: string, value: T): Q.Promise<T>;
        set<T>(key: string, value: T, policy: ICachePolicy): Q.Promise<T>;
        clear(key: string): Q.Promise<void>;
        fullClear(): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    class ControllerFactory {
        private static _controllerRegistry;
        static createController(options: ICreateControllerOptions): IController;
    }
}

declare module Orckestra.Composer {
    interface IComposerTemplates {
        Templates: IHashTable<Function>;
    }
}

declare module Orckestra.Composer {
    interface IRegisterActionOptions {
        actionName: string;
        actionDelegate: Function;
        overwrite?: boolean;
    }
}

declare module Orckestra.Composer {
    interface IControllerActionContext {
        elementContext: JQuery;
        event: JQueryEventObject;
    }
}

declare module Orckestra.Composer {
    interface IParsleyJqueryPlugin extends JQuery {
        parsley(options?: any): any;
    }
}

declare module Orckestra.Composer {
    interface IParsley {
        isValid(group?: any, force?: boolean): boolean;
        validate(group?: any, force?: boolean): boolean;
        destroy(): void;
    }
}

declare module Orckestra.Composer {
    interface UIBusyParam {
        callback?: Function;
        elementContext?: JQuery;
        containerContext?: JQuery;
        loadingIndicatorSelector?: string;
        msDelay?: number;
    }
}

declare module Orckestra.Composer {
    class UIBusyHandle {
        protected loadingIndicatorContext: JQuery;
        protected containerContext: JQuery;
        protected timeoutHandle: any;
        constructor(loadingIndicatorContext: JQuery, containerContext: JQuery, msDelay: number);
        done(): void;
        private startBusy(msDelay);
        private endBusy();
    }
}

declare module Orckestra.Composer {
    class Controller implements IController {
        protected context: IControllerContext;
        protected eventHub: IEventHub;
        protected composerContext: IComposerContext;
        protected composerConfiguration: IComposerConfiguration;
        private _composerEventPostfix;
        private _defaultEventsToMonitor;
        private _unregister;
        protected eventsToMonitor: string[];
        constructor(context: IControllerContext, eventHub: IEventHub, composerContext: IComposerContext, composerConfiguration: IComposerConfiguration);
        static registerAction(classToRegisterActionOn: any, registerActionOptions: IRegisterActionOptions): void;
        initialize(): void;
        dispose(): void;
        asyncBusy(options?: UIBusyParam): UIBusyHandle;
        preventFormSubmit(context: IControllerActionContext): void;
        protected render(templateId: string, viewModel: any, parentSelector?: string): void;
        protected getRenderedTemplateContents(templateId: string, viewModel: any): string;
        protected registerFormsForValidation(context: JQuery, customOptions?: any): Orckestra.Composer.IParsley[];
        private hideServerValidationMessageOnClientValidation(formValidators, serverValidationContainer);
        private registerDomEvents();
        private unregisterDomEvents();
        private parseAction(e);
        private applyControllerAction(context, e);
        private applyControllerActions(context, e, controllerActions);
    }
}

declare module Orckestra.Composer {
    interface ILocalizationProvider {
        initialize(composerContext: IComposerContext): Q.Promise<any>;
        getLocalizedString(categoryName: string, keyName: string): string;
    }
}

declare module Orckestra.Composer {
    class ComposerClient {
        static get(url: string, data?: any): Q.Promise<any>;
        static post(url: string, data: any): Q.Promise<any>;
        static put(url: string, data: any): Q.Promise<any>;
        static remove(url: string, data: any): Q.Promise<any>;
        private static sendRequest(method, url, data?);
        private static getPageCulture();
        private static getWebsiteId();
        private static onRequestRejected(reason);
        private static getReloadUrl();
        private static doesUrlContainQueryString(url, value);
        private static getAjaxFailedErrorMessage();
        private static getUnauthorizedErrorMessage();
        static prepareBloodhound(query: any, settings: any): any;
    }
}

declare module Orckestra.Composer {
    class LocalizationProvider implements Orckestra.Composer.ILocalizationProvider {
        private static _instance;
        private _localizationTree;
        private _composerContext;
        static instance(): Orckestra.Composer.ILocalizationProvider;
        constructor();
        initialize(composerContext: IComposerContext): Q.Promise<any>;
        getLocalizedString(categoryName: string, keyName: string): any;
        protected handleBarsHelper_localize(categoryName: string, keyName: string): any;
        protected handleBarsHelper_localizeFormat(categoryName: string, keyName: string, options: any[]): any;
        protected handleBarsHelper_isLocalized(categoryName: string, keyName: string): boolean;
        private stringFormat(format, options);
    }
}

interface IParsleyValidator extends Window {
    ParsleyValidator: any;
    ParsleyConfig: any;
}

declare module Orckestra.Composer {
    class ComposerContext implements IComposerContext {
        language: string;
    }
}

declare module Orckestra.Composer {
    interface IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
    }
}

declare module Orckestra.Composer {
    class EventHub implements Orckestra.Composer.IEventHub {
        private _events;
        private static _instance;
        static instance(): Orckestra.Composer.IEventHub;
        constructor();
        subscribe(eventName: string, listener: IListener): ISubscription;
        publish(eventName: string, eventInformation: IEventInformation): void;
    }
}

declare module Orckestra.Composer {
    var bootstrap: (window: Window, document: HTMLDocument, composerConfiguration: IComposerConfiguration) => void;
}

declare module Orckestra.Composer {
    interface IPopOverJqueryPlugin extends JQuery {
        popover(options?: any): any;
    }
}

declare module Orckestra.Composer {
    class SearchBoxController extends Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    interface IFacet {
        isArray: boolean;
        facetFieldName: string;
        facetValue: string;
        facetType: string;
        facetLandingPageUrl: string;
    }
}

declare module Orckestra.Composer {
    interface ISelectedFacet {
        facetFieldName: string;
        selectedValues: string[];
        isFacetArray: boolean;
    }
}

declare module Orckestra.Composer {
    interface ISearchCriteriaOptions {
        facetRegistry: IHashTable<string>;
        correctedSearchTerm?: string;
    }
}

declare module Orckestra.Composer {
    class SearchCriteria {
        private eventHub;
        private _window;
        private static facetFieldNameKeyPrefix;
        private static facetValueKeyPrefix;
        private _facetRegistry;
        keywords: string;
        correctedSearchTerm: string;
        page: number;
        sortBy: string;
        sortDirection: string;
        selectedFacets: IHashTable<string | string[]>;
        constructor(eventHub: any, _window: Window);
        initialize(options: ISearchCriteriaOptions): void;
        loadFromQuerystring(querystring: string): void;
        toQuerystring(): string;
        clearFacets(): void;
        addSingleFacet(facetKey: string, facetValue: string): void;
        updateMultiFacets(facets: IHashTable<string | string[]>): void;
        removeFacet(facetToRemove: Orckestra.Composer.IFacet): void;
        private getSelectedFacetsArray(facetFieldName);
        private setSelectedFacet(selectedFacet);
        private clearSelectedMultiFacets();
        private resetPaging();
        private loadFacetCriteria(querystring);
        private loadNonFacetCriteria(querystring);
        private encodeQuerystringValue(valueToEncode);
        private decodeQuerystringValue(valueToDecode);
    }
}

declare module Orckestra.Composer {
    interface ISearchService {
        initialize(options: ISearchCriteriaOptions): any;
        singleFacetsChanged(eventInformation: IEventInformation): any;
        multiFacetChanged(eventInformation: IEventInformation): any;
        clearFacets(eventInformation: IEventInformation): any;
        removeFacet(eventInformation: IEventInformation): any;
        getSelectedFacets(): IHashTable<string | string[]>;
    }
}

declare module Orckestra.Composer {
    interface ISingleSelectCategory {
        isArray: boolean;
        categoryUrl: string;
    }
}

declare module Orckestra.Composer {
    class SearchService implements ISearchService {
        private _eventHub;
        private _window;
        private _searchCriteria;
        private _baseSearchUrl;
        private _baseUrl;
        private _facetRegistry;
        constructor(_eventHub: IEventHub, _window: Window);
        initialize(options: ISearchCriteriaOptions): void;
        singleFacetsChanged(eventInformation: IEventInformation): void;
        sortingChanged(eventInformation: IEventInformation): void;
        getSelectedFacets(): IHashTable<string | string[]>;
        multiFacetChanged(eventInformation: IEventInformation): void;
        clearFacets(eventInformation: IEventInformation): void;
        removeFacet(eventInformation: IEventInformation): void;
        addSingleSelectCategory(eventInformation: IEventInformation): void;
        private registerSubscriptions();
        private search();
    }
}

declare module Orckestra.Composer {
    class AutocompleteSearchService extends SearchService {
        private categoryFacet;
        initialize(options: Orckestra.Composer.ISearchCriteriaOptions): void;
        singleFacetsChanged(eventInformation: Orckestra.Composer.IEventInformation): void;
        removeCategories(): void;
        categorySuggestionClicked(eventInformation: Orckestra.Composer.IEventInformation): void;
        brandSuggestionClicked(eventInformation: Orckestra.Composer.IEventInformation): void;
    }
}

declare module Orckestra.Composer {
    class AutocompleteSearchBoxController extends SearchBoxController {
        private renderedSuggestions;
        private searchService;
        private searchTerm;
        initialize(): void;
        private getBloodhoundInstance(name, limit, url, collectionName?);
        private getDataSetInst(name, bloodhound, template, templateEmpty);
        private resultsNotFound(evt);
        selectedProduct(actionContext: Orckestra.Composer.IControllerActionContext): void;
        selectedSearchTermsSuggestion(actionContext: Orckestra.Composer.IControllerActionContext): void;
        selectedCategorySuggestion(actionContext: Orckestra.Composer.IControllerActionContext): void;
        selectedBrandSuggestion(actionContext: Orckestra.Composer.IControllerActionContext): void;
        showMoreResults(): void;
    }
}

declare module Orckestra.Composer {
    class PageNotFoundAnalyticsController extends Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    class LanguageSwitchController extends Controller {
        private languageSwitchEvent;
        private cacheProvider;
        initialize(): void;
        onLanguageSwitch(): void;
    }
}

declare module Orckestra.Composer {
    class LazyController extends Controller {
        initialize(): void;
        loadContent(): void;
        private replaceContent(newContent);
    }
}

declare module Orckestra.Composer {
    interface ISerializeObjectJqueryPlugin extends JQuery {
        serializeObject(): any;
    }
}

declare module Orckestra.Composer {
    class UrlHelper {
        static resolvePageType(): string;
    }
}

declare module Orckestra.Composer {
    class SortBySearchController extends Orckestra.Composer.Controller {
        sortingChanged(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class SliderService implements IDisposable {
        private context;
        protected eventHub: IEventHub;
        private sliderInstance;
        private facetFieldName;
        private maxLabel;
        private maxValue;
        private minValue;
        private step;
        private applyButtonContext;
        constructor(context: JQuery, eventHub: IEventHub);
        initialize(selectedValues: any): void;
        dispose(): void;
        protected mapData(containerData: any): void;
        dirtied(): void;
        formatFrom(value: any): any;
        formatTo(value: any): any;
        private initializeSlider(facetData);
        private createSlider(startRange, sliderElement);
        getKey(): string;
        getValues(): any[];
    }
}

declare module Orckestra.Composer {
    class FacetSearchController extends Orckestra.Composer.Controller {
        private _debounceHandle;
        private _debounceTimeout;
        private _searchService;
        private sliderService;
        private sliderServicesInstances;
        initialize(): void;
        multiFacetChanged(actionContext: IControllerActionContext): void;
        dispose(): void;
        singleFacetChanged(actionContext: IControllerActionContext): void;
        toggleFacetList(actionContext: IControllerActionContext): void;
        refineByRange(actionContext: IControllerActionContext): void;
        private initializeServices();
        private buildFacetRegistry();
    }
}

declare module Orckestra.Composer {
    interface IError {
        ErrorCode: string;
        LocalizedErrorMessage: string;
    }
}

declare module Orckestra.Composer {
    interface IErrorCollection {
        Errors: IError[];
    }
}

declare module Orckestra.Composer {
    interface IErrorHandler {
        outputError(error: IError): void;
        outputErrorFromCode(errorCode: string): void;
        removeErrors(): void;
    }
}

declare module Orckestra.Composer {
    class ErrorHandler implements IErrorHandler {
        static instance(): IErrorHandler;
        outputError(error: IError): void;
        outputErrorFromCode(errorCode: string): void;
        protected createErrorFromCode(errorCode: string): IError;
        removeErrors(): void;
        protected publishGenericErrorEvent(error?: IError): void;
        protected createErrorCollection(error?: IError): IErrorCollection;
    }
}

declare module Orckestra.Composer {
    interface ICartRepository {
        getCart(): Q.Promise<any>;
        addLineItem(productId: string, variantId: string, quantity: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        updateLineItem(lineItemId: string, quantity: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        deleteLineItem(lineItemId: string): Q.Promise<any>;
        updateBillingMethodPostalCode(postalCode: string): Q.Promise<any>;
        updateShippingMethodPostalCode(postalCode: string): Q.Promise<any>;
        setCheapestShippingMethod(): Q.Promise<any>;
        addCoupon(couponCode: string): Q.Promise<any>;
        removeCoupon(couponCode: string): Q.Promise<any>;
        clean(): Q.Promise<any>;
        updateCart(param: any): Q.Promise<IUpdateCartResult>;
        completeCheckout(currentStep: number): Q.Promise<ICompleteCheckoutResult>;
    }
    interface IUpdateCartResult {
        HasErrors: boolean;
        NextStepUrl: string;
        Cart: any;
    }
    interface ICompleteCheckoutResult {
        OrderNumber: string;
        CustomerEmail: string;
        NextStepUrl: string;
    }
}

declare module Orckestra.Composer {
    class CartRepository implements ICartRepository {
        getCart(): Q.Promise<any>;
        addLineItem(productId: string, variantId: string, quantity: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        updateLineItem(lineItemId: string, quantity: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        deleteLineItem(lineItemId: string): Q.Promise<any>;
        updateBillingMethodPostalCode(postalCode: string): Q.Promise<any>;
        updateShippingMethodPostalCode(postalCode: string): Q.Promise<any>;
        setCheapestShippingMethod(): Q.Promise<any>;
        addCoupon(couponCode: string): Q.Promise<any>;
        removeCoupon(couponCode: string): Q.Promise<any>;
        clean(): Q.Promise<any>;
        updateCart(param: any): Q.Promise<IUpdateCartResult>;
        completeCheckout(currentStep: number): Q.Promise<ICompleteCheckoutResult>;
    }
}

declare module Orckestra.Composer {
    interface ICacheProvider {
        defaultCache: ICache;
        customCache: ICache;
        localStorage: Storage;
        sessionStorage: Storage;
    }
}

declare module Orckestra.Composer {
    interface IStorage {
        init(): Q.Promise<void>;
        initObjectStore(type: string): Q.Promise<void>;
        get<T>(type: string, id: string): Q.Promise<T>;
        remove(type: string, id: string): Q.Promise<void>;
        fullRemove(type: string): Q.Promise<void>;
        set<T>(type: string, item: IStorageItem<T>): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    interface IStorageItem<T> {
        id: string;
        value: T;
    }
}

declare module Orckestra.Composer {
    enum CacheError {
        NotFound = 0,
        Expired = 1,
    }
}

declare module Orckestra.Composer {
    interface ICacheItem<T> {
        value: T;
        policy: ICachePolicy;
        lastAccessed: number;
    }
}

declare module Orckestra.Composer {
    class StorageBasedCache implements Orckestra.Composer.ICache {
        private _storageInitializing;
        private _storage;
        private _type;
        constructor(storage: IStorage, type: string);
        get<T>(key: string): Q.Promise<T>;
        private validate<T>(key, item);
        private isExpired<T>(item);
        set<T>(key: string, value: T, policy?: ICachePolicy, type?: string): Q.Promise<T>;
        clear(key: string): Q.Promise<void>;
        fullClear(): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    class BackingStorage implements Orckestra.Composer.IStorage {
        private _storage;
        private _isInitialized;
        private _initializedObjectStores;
        constructor(_storage: Storage);
        init(): Q.Promise<void>;
        initObjectStore(type: string): Q.Promise<void>;
        private initObjectStoreImpl(type);
        get<T>(type: string, id: string): Q.Promise<T>;
        private getImpl<T>(type, id);
        remove(type: string, id: string): Q.Promise<void>;
        fullRemove(type: string): Q.Promise<void>;
        private removeImpl(type, id);
        private fullRemoveImpl(type);
        set<T>(type: string, item: IStorageItem<T>): Q.Promise<void>;
        private setImpl<T>(type, item);
        private getObjectStore(type);
        private setObjectStore(type, objectStore);
    }
}

declare module Orckestra.Composer {
    enum StorageType {
        localStorage = 0,
        sessionStorage = 1,
    }
}

declare module Orckestra.Composer {
    class StoragePolyfill {
        static create(windowHandle: Window, storageType: StorageType): Storage;
    }
}

declare module Orckestra.Composer {
    var StorageFactory: {
        create: (storageType: StorageType, window: Window) => Storage;
    };
}

declare module Orckestra.Composer {
    class CacheProvider implements ICacheProvider {
        private static defaultCacheKey;
        private static customCacheKey;
        private static _instance;
        window: Window;
        defaultCache: ICache;
        customCache: ICache;
        localStorage: Storage;
        sessionStorage: Storage;
        static instance(): Orckestra.Composer.ICacheProvider;
        constructor();
        getCache(cacheKey: string): StorageBasedCache;
        getDefaultCache(): ICache;
        getCustomCache(): ICache;
        private getLocalStorage();
        private getSessionStorage();
    }
}

declare module Orckestra.Composer {
    class Utils {
        static scrollToElement(element: JQuery, offsetDiff?: number): void;
        static getWebsiteId(): any;
    }
}

declare module Orckestra.Composer {
    interface ICartService {
        getCart(): Q.Promise<any>;
        getFreshCart(): Q.Promise<any>;
        addLineItem(productId: string, price: string, variantId: string, quantity: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        updateLineItem(lineItemId: string, quantity: number, productId: string, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        deleteLineItem(lineItemId: string, productId: string): Q.Promise<any>;
        updateBillingMethodPostalCode(postalCode: string): Q.Promise<void>;
        updateShippingMethodPostalCode(postalCode: string): Q.Promise<void>;
        setCheapestShippingMethod(): Q.Promise<void>;
        addCoupon(couponCode: string): Q.Promise<void>;
        removeCoupon(couponCode: string): Q.Promise<void>;
        clean(): Q.Promise<void>;
        updateCart(param: any): Q.Promise<IUpdateCartResult>;
        completeCheckout(currentStep: number): Q.Promise<ICompleteCheckoutResult>;
        invalidateCache(): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    class CartService implements ICartService {
        protected static GettingFreshCart: Q.Promise<any>;
        protected cacheKey: string;
        protected cachePolicy: ICachePolicy;
        protected cacheProvider: ICacheProvider;
        protected cartRepository: ICartRepository;
        protected eventHub: IEventHub;
        constructor(cartRepository: ICartRepository, eventHub: IEventHub);
        getCart(): Q.Promise<any>;
        protected canHandle(reason: any): boolean;
        getFreshCart(): Q.Promise<any>;
        addLineItem(productId: string, price: string, variantId?: string, quantity?: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        updateLineItem(lineItemId: string, quantity: number, productId: string, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        deleteLineItem(lineItemId: string, productId: string): Q.Promise<any>;
        updateBillingMethodPostalCode(postalCode: string): Q.Promise<any>;
        updateShippingMethodPostalCode(postalCode: string): Q.Promise<any>;
        setCheapestShippingMethod(): Q.Promise<any>;
        addCoupon(couponCode: string): Q.Promise<any>;
        removeCoupon(couponCode: string): Q.Promise<any>;
        clean(): Q.Promise<any>;
        updateCart(param: any): Q.Promise<IUpdateCartResult>;
        completeCheckout(currentStep: number): Q.Promise<ICompleteCheckoutResult>;
        invalidateCache(): Q.Promise<void>;
        protected getCacheCart(): Q.Promise<any>;
        protected setCartToCache(cart: any): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class ProductIdentifierDto {
        ProductId: string;
        VariantId: string;
        constructor(ProductId: string, VariantId: string);
    }
}

declare module Orckestra.Composer {
    interface IProductService {
        calculatePrice(productId: string, concern: string): any;
        getSelectedVariantViewModel(): any;
        getVariant(variantId: string): any;
        updateSelectedKvasWith(selectionsToAdd: any, concern: string): any;
        getRelatedProducts(relatedProductIdentifiers: ProductIdentifierDto[]): any;
        loadQuickBuyProduct(productId: string, variantId: string, concern: string, source: string): any;
        findInventoryItems(viewModel: any, concern: string): any;
        buildUrlPath(pathArray: string[]): string;
    }
}

declare module Orckestra.Composer {
    class ProductService implements IProductService {
        private eventHub;
        private context;
        constructor(eventHub: IEventHub, context: Orckestra.Composer.IControllerContext);
        showQuickView(): void;
        closeQuickView(): void;
        calculatePrice(productId: string, concern: string): Q.Promise<void>;
        getSelectedVariantViewModel(): any;
        getVariant(variantId: string): any;
        updateSelectedKvasWith(selectionsToAdd: any, concern: string): void;
        getRelatedProducts(relatedProductIdentifiers: ProductIdentifierDto[]): Q.Promise<any>;
        loadQuickBuyProduct(productId: string, variantId: string, concern: string, source: string): Q.Promise<any>;
        findInventoryItems(viewModel: any, concern: string): Q.Promise<void>;
        productAvailableToSell(selectedSku: string, productAvailableToSell: string[], productIsAvailableToSell: boolean): boolean;
        private buildKeyVariantAttributeItems(concern);
        replaceHistory(): void;
        buildUrlPath(pathArray: string[]): string;
    }
}

declare module Orckestra.Composer {
    class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: CartService;
        protected productService: ProductService;
        protected currentPage: any;
        initialize(): void;
        private getCurrentPage();
        addToCart(actionContext: IControllerActionContext): void;
        protected onAddToCartFailed(reason: any): void;
        searchProductClick(actionContext: IControllerActionContext): void;
        pagerPageChanged(actionContext: IControllerActionContext): void;
        protected getProductDataForAnalytics(productId: string, price: any): any;
    }
}

declare module Orckestra.Composer {
    class SearchSummaryController extends Orckestra.Composer.Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    interface IWishListRepository {
        getWishListSummary(): Q.Promise<any>;
        getWishList(): Q.Promise<any>;
        addLineItem(productId: string, variantId: string, quantity: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<void>;
        deleteLineItem(lineItemId: string): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    interface IWishListService {
        addLineItem(productId: string, variantId?: string, quantity?: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        removeLineItem(lineItemId: string): Q.Promise<any>;
        getWishListSummary(): Q.Promise<any>;
        clearCache(): any;
    }
}

declare module Orckestra.Composer {
    class WishListService implements IWishListService {
        private static GettingFreshWishListSummary;
        private wishListRepository;
        private cacheKey;
        private cachePolicy;
        private cacheProvider;
        private eventHub;
        constructor(wishListRepository: IWishListRepository, eventHub: IEventHub);
        getWishListSummary(): Q.Promise<any>;
        getFreshWishListSummary(): Q.Promise<any>;
        getSignInUrl(): Q.Promise<any>;
        getLineItem(productId: string, variantId?: string): Q.Promise<any>;
        addLineItem(productId: string, variantId?: string, quantity?: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        removeLineItem(lineItemId: string): Q.Promise<any>;
        clearCache(): Q.Promise<void>;
        protected getCacheWishListSummary(): Q.Promise<any>;
        protected setWishListToCache(wishList: any): Q.Promise<any>;
        private canHandle(reason);
    }
}

declare module Orckestra.Composer {
    class WishListRepository implements Orckestra.Composer.IWishListRepository {
        getWishList(): Q.Promise<any>;
        getWishListSummary(): Q.Promise<any>;
        addLineItem(productId: string, variantId: string, quantity: number, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<void>;
        deleteLineItem(lineItemId: string): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    interface IMembershipRepository {
        login(formData: any, returnUrl: string): Q.Promise<any>;
        logout(returnUrl: string, preserveCustomerInfo: boolean): Q.Promise<any>;
        register(formData: any, returnUrl: string): Q.Promise<any>;
        forgotPassword(formData: any): Q.Promise<any>;
        resetPassword(formData: any, ticket: string, returnUrl: string): Q.Promise<any>;
        changePassword(formData: any, returnUrl: string): Q.Promise<any>;
        isAuthenticated(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class MembershipRepository implements IMembershipRepository {
        login(formData: any, returnUrl: string): Q.Promise<any>;
        logout(returnUrl?: string, preserveCustomerInfo?: boolean): Q.Promise<any>;
        register(formData: any, returnUrl: string): Q.Promise<any>;
        forgotPassword(formData: any): Q.Promise<any>;
        resetPassword(formData: any, ticket: string, returnUrl: string): Q.Promise<any>;
        changePassword(formData: any, returnUrl: string): Q.Promise<any>;
        isAuthenticated(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface IMembershipService {
        login(formData: any, returnUrl: string): Q.Promise<any>;
        logout(returnUrl: string, preserveCustomerInfo: boolean): Q.Promise<any>;
        register(formData: any, returnUrl: string): Q.Promise<any>;
        forgotPassword(formData: any): Q.Promise<any>;
        resetPassword(formData: any, ticket: string, returnUrl: string): Q.Promise<any>;
        changePassword(formData: any, returnUrl: string): Q.Promise<any>;
        isAuthenticated(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class MembershipService implements IMembershipService {
        protected memoizeIsAuthenticated: Function;
        protected membershipRepository: IMembershipRepository;
        constructor(membershipRepository: IMembershipRepository);
        login(formData: any, returnUrl: string): Q.Promise<any>;
        logout(returnUrl?: string, preserveCustomerInfo?: boolean): Q.Promise<any>;
        register(formData: any, returnUrl: string): Q.Promise<any>;
        forgotPassword(formData: any): Q.Promise<any>;
        resetPassword(formData: any, ticket: string, returnUrl: string): Q.Promise<any>;
        changePassword(formData: any, returnUrl: string): Q.Promise<any>;
        isAuthenticated(): Q.Promise<any>;
        protected isAuthenticatedImpl(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface IInventoryService {
        isAvailableToSell(sku: string): Q.Promise<boolean>;
    }
}

declare module Orckestra.Composer {
    class InventoryService implements IInventoryService {
        private _memoizeIsAvailableToSell;
        isAvailableToSell(sku: string): Q.Promise<boolean>;
        private isAvailableToSellImpl(sku);
    }
}

declare module Orckestra.Composer {
    class ProductFormatter {
        convertToStronglyTyped(strValue: string, propertyDataType: string): any;
    }
}

declare module Orckestra.Composer {
    class KeyVariantAttributeItemsBuilder {
        private context;
        constructor(context: Orckestra.Composer.IControllerContext);
        BuildKeyVariantAttributeItemsFor(selectedKvas: any): any;
        private InitiateKVAStateFor(keyVariantAttributeItems, selectedKvas);
        private FindReachableVariantsFrom(keyVariantAttributeItems, selectedKvas);
        private EnableKVAState(reverseKvaLookup, reachableVariants, selectedKvas);
    }
}

declare module Orckestra.Composer {
    class ProductController extends Orckestra.Composer.Controller {
        protected inventoryService: InventoryService;
        protected productService: ProductService;
        protected cartService: CartService;
        protected _wishListService: WishListService;
        protected _membershipService: IMembershipService;
        protected concern: string;
        initialize(): void;
        protected registerSubscriptions(): void;
        protected onSelectedVariantIdChanged(e: IEventInformation): void;
        protected onSelectedKvasChanged(e: IEventInformation): void;
        protected onImagesChanged(e: IEventInformation): void;
        protected onPricesChanged(e: IEventInformation): void;
        protected renderData(): Q.Promise<void[]>;
        protected isProductWithVariants(): boolean;
        protected isSelectedVariantUnavailable(): boolean;
        protected renderUnavailableQuantity(quantity: any): Q.Promise<void>;
        protected renderAvailableQuantity(quantity: any): Q.Promise<void>;
        protected renderAddToWishList(): Q.Promise<void>;
        protected renderUnavailableAddToCart(): Q.Promise<void>;
        protected renderAvailableAddToCart(): Q.Promise<void>;
        decrementQuantity(actionContext: IControllerActionContext): void;
        incrementQuantity(actionContext: IControllerActionContext): void;
        changeQuantity(actionContext: IControllerActionContext): void;
        addLineItemToWishList(actionContext: IControllerActionContext): void;
        removeLineItemToWishList(actionContext: IControllerActionContext): void;
        protected redirectToSignInBeforeAddToWishList(): void;
        addLineItem(actionContext: IControllerActionContext, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): void;
        protected onAddLineItemSuccess(data: any): void;
        protected onAddLineItemFailed(reason: any): void;
        protected getCurrentQuantity(): {
            Min: number;
            Max: number;
            Value: number;
        };
        protected addLineItemImpl(productId: string, price: string, variantId: string, quantity: any, recurringOrderFrequencyName?: string, recurringOrderProgramName?: string): Q.Promise<any>;
        protected completeAddLineItem(quantityAdded: any): Q.Promise<void>;
        selectImage(actionContext: IControllerActionContext): void;
        selectKva(actionContext: IControllerActionContext): void;
        protected calculatePrice(): Q.Promise<any>;
        protected getProductDataForAnalytics(vm: any): any;
        protected getListNameForAnalytics(): string;
        protected getVariantDataForAnalytics(variant: any): any;
        protected buildVariantName(kvas: any): string;
    }
}

declare module Orckestra.Composer {
    interface IQuickViewController {
        selectImage(actionContext: IControllerActionContext): void;
        addLineItem(actionContext: IControllerActionContext): void;
        selectKva(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class QuickViewController extends Orckestra.Composer.ProductController implements Orckestra.Composer.IQuickViewController {
        protected concern: string;
        private selectedRecurringOrderFrequencyName;
        private recurringMode;
        initialize(): void;
        protected setConcernWithContext(): void;
        protected registerSubscriptions(): void;
        protected onQuickBuyLoaded(e: IEventInformation): void;
        protected onLoadingFailed(reason: any): void;
        private setVariantId(variantId);
        protected onSelectedVariantIdChanged(e: IEventInformation): void;
        protected onSelectedVariantIdChangedSuccess(): void;
        protected onSelectedVariantIdChangedFailed(reason: any): void;
        protected onSelectedKvasChanged(e: IEventInformation): void;
        protected onImagesChanged(e: IEventInformation): void;
        private getUnavailableMainImageContent(e);
        protected onPricesChanged(e: IEventInformation): void;
        protected renderUnavailableAddToCart(): Q.Promise<void>;
        protected renderAvailableAddToCart(): Q.Promise<void>;
        protected completeAddLineItem(quantityAdded: any): Q.Promise<void>;
        protected getListNameForAnalytics(): string;
        private renderRecurringAddToCartProductDetailFrequency();
        onRecurringOrderFrequencySelectChanged(actionContext: IControllerActionContext): void;
        changeRecurringMode(actionContext: IControllerActionContext): void;
        addToCartButtonClick(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class SelectedFacetSearchController extends Orckestra.Composer.Controller {
        initialize(): void;
        removeSelectedFacet(actionContext: IControllerActionContext): void;
        clearSelectedFacets(actionContext: IControllerActionContext): void;
        addSingleSelectCategory(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class AddToCartNotificationController extends Controller {
        initialize(): void;
        private registerSubscriptions();
        displayNotification(e: IEventInformation): void;
        onClose(e: IControllerActionContext): void;
        private closeNotification();
    }
}

declare module Orckestra.Composer {
    interface IScheduledCallback {
        (data: any): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class EventScheduler {
        private static instances;
        private eventName;
        private onEventCallbacks;
        private postEventCallback;
        static instance(eventName: string): EventScheduler;
        constructor(eventName: string);
        subscribe(callback: IScheduledCallback): void;
        setPostEventCallback(postEventCallback: IScheduledCallback): void;
        private trigger(data);
        private triggerCallbacks(data);
        private triggerPostEvent(data);
        protected onError(reason: any): void;
    }
}

declare module Orckestra.Composer {
    interface IAddRecurringOrderCartLineItemParam {
        cartName: string;
        productId: string;
        productDisplayName: string;
        variantId: string;
        sku: string;
        quantity: number;
        recurringOrderFrequencyName: string;
        recurringOrderProgramName: string;
    }
    interface IRecurringOrderCartUpdateShippingMethodParam {
        shippingProviderId: string;
        shippingMethodName: string;
        cartName: string;
    }
    interface IRecurringOrderGetCartShippingMethods {
        CartName: string;
    }
    interface IRecurringOrderLineItemDeleteParam {
        lineItemId: string;
        cartName: string;
    }
    interface IRecurringOrderLineItems {
        RecurringOrderTemplateLineItemId: string;
    }
    interface IRecurringOrderLineItemsDeleteParam {
        lineItemsIds: string[];
        cartName: string;
    }
    interface IRecurringOrderLineItemsUpdateDateParam {
        CartName: string;
        NextOccurence: string;
    }
    interface IRecurringOrderProgramsByNamesParam {
        recurringOrderProgramNames: string[];
    }
    interface IRecurringOrderTemplateLineItemDeleteParam {
        lineItemId: string;
    }
    interface IRecurringOrderTemplateLineItemUpdateParam {
        paymentMethodId: string;
        shippingAddressId: string;
        billingAddressId: string;
        lineItemId: string;
        nextOccurence: Date;
        frequencyName: string;
        shippingProviderId: string;
        shippingMethodName: string;
    }
    interface IRecurringOrderTemplateLineItemsDeleteParam {
        lineItemsIds: string[];
    }
    interface IRecurringOrderTemplateUpdateLineItemQuantityParam {
        lineItemId: string;
        quantity: number;
    }
    interface IRecurringOrderUpdateLineItemQuantityParam {
        lineItemId: string;
        quantity: number;
        cartName: string;
        recurringProgramName: string;
        recurringFrequencyName: string;
    }
    interface IRecurringOrderUpdateTemplateAddressParam {
        shippingAddressId: string;
        billingAddressId: string;
        cartName: string;
        useSameForShippingAndBilling: boolean;
    }
    interface IRecurringOrderUpdateTemplatePaymentMethodParam {
        paymentMethodId: string;
        cartName: string;
        providerName: string;
    }
    interface IRecurringOrderCartParam {
        cartName: string;
    }
    interface IRecurringOrderUpdateCartAddressParam {
        shippingAddressId: string;
        billingAddressId: string;
        cartName: string;
        useSameForShippingAndBilling: boolean;
    }
    interface IRecurringOrderGetCartPaymentMethods {
        cartName: string;
    }
    interface IRecurringOrderCartUpdatePaymentMethodParam {
        cartName: string;
        paymentId: string;
        paymentProviderName: string;
        paymentType: string;
        paymentMethodId: string;
    }
    interface IRecurringOrderGetTemplatePaymentMethods {
        id: string;
    }
}

declare module Orckestra.Composer {
    interface IRecurringOrderService {
        updateLineItemsDate(updateLineItemsParam: IRecurringOrderLineItemsUpdateDateParam): Q.Promise<any>;
        deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam): Q.Promise<any>;
        deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam): Q.Promise<any>;
        getCustomerAddresses(): Q.Promise<any>;
        getCustomerPaymentMethods(): Q.Promise<any>;
        updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateCartAddressParam): Q.Promise<any>;
        updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any>;
        updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Q.Promise<any>;
        updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam): Q.Promise<any>;
        getRecurringOrderCartsByUser(): Q.Promise<any>;
        getRecurringOrderTemplatesByUser(): Q.Promise<any>;
        updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam): Q.Promise<any>;
        getRecurringOrderProgramsByUser(): Q.Promise<any>;
        getRecurringOrderProgramsByNames(programsByNamesParam: IRecurringOrderProgramsByNamesParam): Q.Promise<any>;
        updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Q.Promise<any>;
        deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Q.Promise<any>;
        deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Q.Promise<any>;
        getCartContainsRecurrence(): Q.Promise<any>;
        getRecurrenceConfigIsActive(): Q.Promise<any>;
        getCanRemovePaymentMethod(paymentMethodId: string): Q.Promise<any>;
        getRecurringOrderCartSummaries(): Q.Promise<any>;
        addRecurringOrderCartLineItem(addRecurringOrderCartLineItemParam: IAddRecurringOrderCartLineItemParam): Q.Promise<any>;
        getAnonymousCartSignInUrl(): Q.Promise<any>;
        updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Q.Promise<any>;
        getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Q.Promise<any>;
        getOrderTemplateShippingMethods(): Q.Promise<any>;
        getInactifProductsFromCustomer(): Q.Promise<any>;
        clearCustomerInactifItems(): Q.Promise<any>;
        getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Q.Promise<any>;
        getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Q.Promise<any>;
        updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Q.Promise<any>;
        getRecurringTemplateDetail(recurringOrderTemplateId: string): Q.Promise<any>;
        getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class RecurringOrderService implements IRecurringOrderService {
        protected repository: IRecurringOrderRepository;
        protected eventHub: Orckestra.Composer.IEventHub;
        constructor(repository: IRecurringOrderRepository, eventHub: Orckestra.Composer.IEventHub);
        updateLineItemsDate(updateLineItemsParam: IRecurringOrderLineItemsUpdateDateParam): Q.Promise<any>;
        deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam): Q.Promise<any>;
        deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam): Q.Promise<any>;
        getCustomerAddresses(): Q.Promise<any>;
        getCustomerPaymentMethods(): Q.Promise<any>;
        updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateCartAddressParam): Q.Promise<any>;
        updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any>;
        updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Q.Promise<any>;
        updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam): Q.Promise<any>;
        getRecurringOrderCartsByUser(): Q.Promise<any>;
        getRecurringOrderTemplatesByUser(): Q.Promise<any>;
        updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam): Q.Promise<any>;
        getRecurringOrderProgramsByUser(): Q.Promise<any>;
        getRecurringOrderProgramsByNames(programsByNamesParam: IRecurringOrderProgramsByNamesParam): Q.Promise<any>;
        updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Q.Promise<any>;
        deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Q.Promise<any>;
        deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Q.Promise<any>;
        getCartContainsRecurrence(): Q.Promise<any>;
        getRecurrenceConfigIsActive(): Q.Promise<any>;
        getCanRemovePaymentMethod(paymentMethodId: string): Q.Promise<any>;
        getRecurringOrderCartSummaries(): Q.Promise<any>;
        addRecurringOrderCartLineItem(addLineItemQuantityParam: IAddRecurringOrderCartLineItemParam): Q.Promise<any>;
        getAnonymousCartSignInUrl(): Q.Promise<any>;
        updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Q.Promise<any>;
        getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Q.Promise<any>;
        getOrderTemplateShippingMethods(): Q.Promise<any>;
        getInactifProductsFromCustomer(): Q.Promise<any>;
        clearCustomerInactifItems(): Q.Promise<any>;
        getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Q.Promise<any>;
        getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Q.Promise<any>;
        updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Q.Promise<any>;
        getRecurringTemplateDetail(recurringOrderTemplateId: string): Q.Promise<any>;
        getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Q.Promise<any>;
        private _mapLineItemToRequest(lineItem);
        private _mapRecurringLineItemToRequest(lineItem);
    }
}

declare module Orckestra.Composer {
    interface IRecurringOrderProgramFrequencyInfoViewModel {
        RecurringOrderProgramFrequencyId: string;
        RecurrenceDefDescription: string;
    }
    interface IRecurringOrderProgramEligibleItemResultViewModel {
        CategoryId: string;
        ProductId: string;
        Sku: string;
        VariantId: string;
        IsEligible: boolean;
        Frequencies: Array<IRecurringOrderProgramFrequencyInfoViewModel>;
    }
    interface IRecurringFrequecyDescriptionListViewModel {
        Frequencies: Array<IRecurringOrderProgramFrequencyInfoViewModel>;
    }
    interface IRecurringOrderProgramEligibleItemListViewModel {
        Items: Array<IRecurringOrderProgramEligibleItemResultViewModel>;
    }
}

declare module Orckestra.Composer {
    interface IRecurringOrderRepository {
        updateLineItemsDate(updateLineItemParam: IRecurringOrderLineItemsUpdateDateParam): Q.Promise<any>;
        deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam): Q.Promise<any>;
        deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam): Q.Promise<any>;
        getCustomerAddresses(): Q.Promise<any>;
        getCustomerPaymentMethods(): Q.Promise<any>;
        updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateCartAddressParam): Q.Promise<any>;
        updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any>;
        updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Q.Promise<any>;
        updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam): Q.Promise<any>;
        getRecurringOrderCartsByUser(): Q.Promise<any>;
        getRecurringOrderTemplatesByUser(): Q.Promise<any>;
        updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam): Q.Promise<any>;
        getRecurringOrderProgramsByUser(): Q.Promise<any>;
        getRecurringOrderProgramsByNames(programsByNamesParam: IRecurringOrderProgramsByNamesParam): Q.Promise<any>;
        updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Q.Promise<any>;
        deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Q.Promise<any>;
        deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Q.Promise<any>;
        getCartContainsRecurrence(): Q.Promise<any>;
        getRecurrenceConfigIsActive(): Q.Promise<any>;
        getCanRemovePaymentMethod(paymentMethodId: string): Q.Promise<any>;
        getRecurringOrderCartSummaries(): Q.Promise<any>;
        addRecurringOrderCartLineItem(addRecurringOrderCartLineItemParam: IAddRecurringOrderCartLineItemParam): Q.Promise<any>;
        getAnonymousCartSignInUrl(): Q.Promise<any>;
        updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Q.Promise<any>;
        getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Q.Promise<any>;
        getOrderTemplateShippingMethods(): Q.Promise<any>;
        getInactifProductsFromCustomer(): Q.Promise<any>;
        clearCustomerInactifItems(): Q.Promise<any>;
        getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Q.Promise<any>;
        getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Q.Promise<any>;
        updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Q.Promise<any>;
        getRecurringTemplateDetail(recurringOrderTemplateId: string): Q.Promise<any>;
        getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class RecurringOrderRepository implements IRecurringOrderRepository {
        updateLineItemsDate(updateLineItemsParam: IRecurringOrderLineItemsUpdateDateParam): Q.Promise<any>;
        deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam): Q.Promise<any>;
        deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam): Q.Promise<any>;
        getCustomerAddresses(): Q.Promise<any>;
        getCustomerPaymentMethods(): Q.Promise<any>;
        getRecurringOrderCartsByUser(): Q.Promise<any>;
        getRecurringOrderTemplatesByUser(): Q.Promise<any>;
        updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any>;
        updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any>;
        updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Q.Promise<any>;
        updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam): Q.Promise<any>;
        updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam): Q.Promise<any>;
        getRecurringOrderProgramsByUser(): Q.Promise<any>;
        getRecurringOrderProgramsByNames(programsByNamesParam: IRecurringOrderProgramsByNamesParam): Q.Promise<any>;
        updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Q.Promise<any>;
        deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Q.Promise<any>;
        deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Q.Promise<any>;
        getCartContainsRecurrence(): Q.Promise<any>;
        getRecurrenceConfigIsActive(): Q.Promise<any>;
        getCanRemovePaymentMethod(paymentMethodId: any): Q.Promise<any>;
        getRecurringOrderCartSummaries(): Q.Promise<any>;
        addRecurringOrderCartLineItem(addLineItemQuantityParam: IAddRecurringOrderCartLineItemParam): Q.Promise<any>;
        updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Q.Promise<any>;
        getAnonymousCartSignInUrl(): Q.Promise<any>;
        getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Q.Promise<any>;
        getOrderTemplateShippingMethods(): Q.Promise<any>;
        getInactifProductsFromCustomer(): Q.Promise<any>;
        clearCustomerInactifItems(): Q.Promise<any>;
        getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Q.Promise<any>;
        getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Q.Promise<any>;
        updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Q.Promise<any>;
        getRecurringTemplateDetail(recurringOrderTemplateId: string): Q.Promise<any>;
        getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class FullCartController extends Orckestra.Composer.Controller {
        protected source: string;
        protected debounceUpdateLineItem: (args: any) => void;
        protected loaded: boolean;
        protected cartService: CartService;
        initialize(): void;
        protected registerSubscriptions(): void;
        protected onCartUpdated(cart: any): void;
        protected loadCart(): void;
        protected loadCartFailed(reason: any): void;
        updateLineItem(actionContext: IControllerActionContext): void;
        protected executeLineItemUpdate(args: any): void;
        protected isUpdateRequired(updateLineItemArgs: any, lineItem: any): boolean;
        protected lineItemUpdateFailed(context: JQuery, reason: any): void;
        protected getLineItemDataForAnalytics(lineItem: any, quantity: number): any;
        updateQuantity(action: string, quantity: number): number;
        deleteLineItem(actionContext: IControllerActionContext): void;
        protected onLineItemDeleteFailed(context: JQuery, reason: any): void;
        protected buildVariantName(kvas: any[]): string;
        updateLineItemRecurringFrequency(actionContext: IControllerActionContext): void;
        resetLineItemRecurringFrequency(actionContext: IControllerActionContext): void;
        changeRecurringMode(actionContext: IControllerActionContext): void;
        expandRecurringModes(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    enum MyAccountEvents {
        AccountCreated = 0,
        AccountUpdated = 1,
        AddressCreated = 2,
        AddressUpdated = 3,
        AddressDeleted = 4,
        LoggedIn = 5,
        LoggedOut = 6,
        PasswordChanged = 7,
        ForgotPasswordInstructionSent = 8,
    }
}

declare module Orckestra.Composer {
    class MiniCartController extends Orckestra.Composer.Controller {
        private cartService;
        initialize(): void;
        private initializeMiniCartQuantity();
        protected registerSubscriptions(): void;
        protected onCartUpdated(e: IEventInformation): void;
        protected onRefreshUser(e: IEventInformation): Q.Promise<any>;
        protected renderCart(cart: any): void;
        protected onError(reason: any): void;
    }
}

declare module Orckestra.Composer {
    interface IAnalyticsProduct {
        id?: string;
        name?: string;
        price?: number;
        brand?: string;
        category?: string;
        variant?: string;
        quantity?: number;
        position?: number;
        list?: string;
    }
}

declare module Orckestra.Composer {
    interface IAnalyticsSearchResults {
        ProductSearchResults?: any;
        keywordEntered?: string;
        keywordCorrected?: string;
    }
}

declare module Orckestra.Composer {
    interface IAnalyticsPlugin {
        productImpressions(products: IAnalyticsProduct[]): void;
        productClick(product: IAnalyticsProduct, listName: string): void;
        productDetailImpressions(products: IAnalyticsProduct[], listName: string): void;
        addToCart(product: IAnalyticsProduct, listName: string): void;
        removeFromCart(product: IAnalyticsProduct, listName: string): void;
        checkout(step: number, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        checkoutOption(step: number): void;
        searchKeywordCorrection(searchResults: IAnalyticsSearchResults): void;
        noResultsFound(keywordNotFound: string): void;
        couponsUsed(order: IAnalyticsCoupon): void;
        purchase(order: IAnalyticsOrder, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        sendEvent(eventName: string, category: string, action: string, label?: string, value?: number): void;
        userLoggedIn(type: string, source: string): void;
        userCreated(): void;
        recoverPassword(): void;
    }
}

declare module Orckestra.Composer {
    interface IAnalyticsCoupon {
        code?: string;
        discountAmount?: string;
        currencyCode?: string;
        promotionName?: string;
    }
}

declare module Orckestra.Composer {
    interface IAnalyticsOrder {
        id?: string;
        affiliation?: string;
        revenue?: string;
        tax?: string;
        shipping?: string;
        currencyCode?: string;
        coupon?: string;
    }
}

declare module Orckestra.Composer {
    interface IAnalyticsTransaction {
        shippingType?: string;
        checkoutOrigin?: string;
    }
}

declare module Orckestra.Composer {
    interface IAnalyticsSearchFilters {
        facetKey?: string;
        facetValue?: string;
        pageType?: string;
    }
}

declare module Orckestra.Composer {
    class AnalyticsPlugin implements IAnalyticsPlugin, IPlugin {
        protected cacheProvider: ICacheProvider;
        protected eventHub: IEventHub;
        protected useVariantIdWhenPossible: boolean;
        initialize(): void;
        static setCheckoutOrigin(checkoutOrigin: string): void;
        static getCheckoutOrigin(): string;
        registerSubscriptions(): void;
        protected onUserLoggedIn(eventInfo: IEventInformation): void;
        protected onUserCreated(eventInfo: IEventInformation): void;
        protected onRecoverPassword(eventInfo: IEventInformation): void;
        protected onSingleFacetChanged(eventInfo: IEventInformation): void;
        protected onMultiFacetChanged(eventInfo: IEventInformation): void;
        protected onSortingChanged(eventInfo: IEventInformation): void;
        protected onPageNotFound(eventInfo: IEventInformation): void;
        protected onWishListCopingShareUrl(eventInfo: IEventInformation): void;
        protected onWishListLineItemAdding(eventInfo: IEventInformation): void;
        protected onLineItemAdding(eventInfo: IEventInformation): void;
        protected onLineItemRemoving(eventInfo: IEventInformation): void;
        protected onProductDetailsRendered(eventInfo: IEventInformation): void;
        protected onCheckoutStepRendered(eventInfo: IEventInformation): void;
        protected onCheckoutNavigationRendered(eventInfo: IEventInformation): void;
        protected onCheckoutCompleted(eventInfo: IEventInformation): void;
        protected onSearchResultRendered(eventInfo: IEventInformation): void;
        protected onRelatedProductsLoaded(eventInfo: IEventInformation): void;
        protected onProductClick(eventInfo: IEventInformation): void;
        protected onNoResultsFound(eventInfo: IEventInformation): void;
        protected onSearchTermCorrected(eventInfo: IEventInformation): void;
        protected buildVariantForLineItem(lineItem: any): string;
        protected buildVariantName(kvas: any[]): string;
        protected mapAnalyticProductsFromLineItems(data: any): IAnalyticsProduct[];
        protected mapAnalyticCouponsFromOrder(data: any): IAnalyticsCoupon[];
        protected mapAnalyticTransactionFromOrder(data: any): IAnalyticsTransaction;
        userLoggedIn(type: string, source: string): void;
        userCreated(): void;
        recoverPassword(): void;
        singleFacetChanged(searchFilters: IAnalyticsSearchFilters): void;
        multiFacetChanged(searchFilters: IAnalyticsSearchFilters): void;
        sortingChanged(sortingType: string, pageType: string): void;
        productImpressions(products: IAnalyticsProduct[]): void;
        productClick(product: IAnalyticsProduct, listName: string): void;
        productDetailImpressions(products: IAnalyticsProduct[], listName: string): void;
        addToCart(product: IAnalyticsProduct, listName: string): void;
        addToWishList(product: IAnalyticsProduct): void;
        removeFromCart(product: IAnalyticsProduct, listName: string): void;
        checkout(step: number, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        checkoutOption(step: number): void;
        purchase(order: IAnalyticsOrder, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        couponsUsed(order: IAnalyticsCoupon): void;
        shareWishList(data: any): void;
        searchKeywordCorrection(data: IAnalyticsSearchResults): void;
        noResultsFound(keywordNotFound: string): void;
        sendEvent(eventName: string, category: string, action: string, label?: string, value?: number): void;
        trimPrice(price: any): number;
        trimPriceAndUnlocalize(price: any): any;
        formatDate(date: any): string;
    }
}

declare var dataLayer: any;
declare var ga: any;
declare module Orckestra.Composer {
    class GoogleAnalyticsPlugin extends AnalyticsPlugin {
        initialize(): void;
        userLoggedIn(type: string, source: string): void;
        userCreated(): void;
        recoverPassword(): void;
        singleFacetChanged(searchFilters: IAnalyticsSearchFilters): void;
        multiFacetChanged(searchFilters: IAnalyticsSearchFilters): void;
        sortingChanged(sortingType: string, pageType: string): void;
        productImpressions(products: IAnalyticsProduct[]): void;
        productClick(product: IAnalyticsProduct, listName: string): void;
        productDetailImpressions(products: IAnalyticsProduct[], listName: string): void;
        addToCart(product: IAnalyticsProduct, listName: string): void;
        addToWishList(product: IAnalyticsProduct): void;
        couponsUsed(coupon: IAnalyticsCoupon): void;
        shareWishList(data: any): void;
        removeFromCart(product: IAnalyticsProduct, listName: string): void;
        checkout(step: number, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        checkoutOption(step: number): void;
        purchase(order: IAnalyticsOrder, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        noResultsFound(keywordNotFound: string): void;
        searchKeywordCorrection(searchResults: IAnalyticsSearchResults): void;
        sendEvent(eventName: string, category: string, action: string, label?: string, value?: number): void;
        private getStepNumber(step);
    }
}

declare module Orckestra.Composer {
    class MiniCartSummaryController extends Orckestra.Composer.Controller {
        private cartService;
        private cacheProvider;
        private timer;
        initialize(): void;
        private registerSubscriptions();
        private invalidateCart(e);
        private displayMiniCart(e);
        private onCloseMiniCart(e);
        protected onCheckout(actionContext: IControllerActionContext): void;
        protected initializeMiniCartSummary(): void;
        protected renderMiniCart(cart: any): void;
    }
}

declare module Orckestra.Composer {
    class CouponService {
        private cartService;
        private eventHub;
        constructor(cartService: ICartService, eventHub: IEventHub);
        addCoupon(couponCode: string): Q.Promise<void>;
        removeCoupon(couponCode: string): Q.Promise<void>;
        private publishCouponUpdatedEvent(result, isSuccess);
    }
}

declare module Orckestra.Composer {
    class CouponController extends Orckestra.Composer.Controller {
        private couponService;
        private isFirstLoad;
        initialize(): void;
        private registerSubscriptions();
        applyCoupon(actionContext: IControllerActionContext): void;
        removeCoupon(actionContext: IControllerActionContext): void;
        private onCouponUpdated(viewModel);
        private getCouponForm();
    }
}

declare module Orckestra.Composer {
    class ProductDetailController extends Orckestra.Composer.ProductController {
        protected concern: string;
        private selectedRecurringOrderFrequencyName;
        private recurringMode;
        initialize(): void;
        protected getListNameForAnalytics(): string;
        protected notifyAnalyticsOfProductDetailsImpression(): void;
        protected publishProductImpressionEvent(data: any): void;
        protected onSelectedVariantIdChanged(e: IEventInformation): void;
        protected handleHiddenImages(el: any): void;
        protected onSelectedKvasChanged(e: IEventInformation): void;
        protected onPricesChanged(e: IEventInformation): void;
        protected renderUnavailableAddToCart(): Q.Promise<void>;
        protected renderAvailableAddToCart(): Q.Promise<void>;
        selectKva(actionContext: IControllerActionContext): void;
        private replaceHistory(previousSelectedVariantId);
        protected completeAddLineItem(quantityAdded: any): Q.Promise<void>;
        private renderRecurringAddToCartProductDetailFrequency();
        onRecurringOrderFrequencySelectChanged(actionContext: IControllerActionContext): void;
        changeRecurringMode(actionContext: IControllerActionContext): void;
        addToCartButtonClick(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class SlickCarouselPlugin implements IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
        initSlick(): void;
        private subscriptEvents();
    }
}

declare module Orckestra.Composer {
    class RelatedProductController extends Orckestra.Composer.ProductController {
        protected concern: string;
        private source;
        private products;
        initialize(): void;
        private getRelatedProducts();
        protected onGetRelatedProductsSuccess(vm: any): void;
        protected onGetRelatedProductsFailed(reason: any): void;
        protected getPageSource(): string;
        protected getListNameForAnalytics(): string;
        protected onLoadingFailed(reason: any): void;
        addToCart(actionContext: IControllerActionContext): void;
        protected addVariantProductToCart(productId: string, variantId: string, price: string): Q.Promise<any>;
        protected addNonVariantProductToCart(productId: string, price: string, recurringProgramName: string): Q.Promise<any>;
        protected getProductViewModel(productId: string): any;
        protected getCurrentQuantity(): any;
        relatedProductsClick(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class ProductZoomController extends Orckestra.Composer.ProductController {
        private allImages;
        initialize(): void;
        protected openZoom(event: JQueryEventObject): void;
        protected changeZoomedImage(event: JQueryEventObject): void;
        protected errorZoomedImage(event: JQueryEventObject): void;
        protected initZoom(): void;
        protected updateModalImages(e: any): void;
    }
}

declare module Orckestra.Composer {
    class OrderSummaryService {
        private cartService;
        private eventHub;
        constructor(cartService: ICartService, eventHub: IEventHub);
        setCheapestShippingMethodUsing(postalCode: string): Q.Promise<void>;
        cleanCart(): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    class OrderSummaryController extends Orckestra.Composer.Controller {
        private cacheProvider;
        private cartService;
        private orderSummaryService;
        private postalCodeModal;
        private postalCodeInput;
        initialize(): void;
        private registerSubscriptions();
        openModal(actionContext: IControllerActionContext): void;
        private clearForm();
        private closeModal(actionContext);
        estimateShipping(actionContext: IControllerActionContext): void;
        proceedToCheckout(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class ShippingMethodService {
        getShippingMethods(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface IBaseCheckoutController extends IController {
        viewModelName: string;
        unregisterController(): any;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        getValidationPromise(): Q.Promise<boolean>;
        getUpdateModelPromise(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface IRegionService {
        getRegions(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class RegionService implements IRegionService {
        private _memoizeGetRegions;
        getRegions(): Q.Promise<any>;
        private getRegionsImpl();
    }
}

declare module Orckestra.Composer {
    interface ICheckoutService {
        registerController(controller: IController): any;
        unregisterController(controllerName: string): any;
        getCart(): Q.Promise<any>;
        updateCart(): Q.Promise<IUpdateCartResult>;
        completeCheckout(): Q.Promise<ICompleteCheckoutResult>;
        updatePostalCode(postalCode: string): Q.Promise<void>;
        invalidateCache(): Q.Promise<void>;
        setOrderConfirmationToCache(orderConfirmationviewModel: any): void;
        getOrderConfirmationFromCache(): Q.Promise<any>;
        clearOrderConfirmationFromCache(): void;
        setOrderToCache(orderConfirmationviewModel: any): void;
    }
}

declare module Orckestra.Composer {
    interface ICheckoutContext {
        authenticationViewModel: IAuthenticationViewModel;
        cartViewModel: any;
        regionsViewModel: any;
        shippingMethodsViewModel: any;
    }
    interface IAuthenticationViewModel {
        IsAuthenticated: boolean;
    }
    interface ICartViewModel {
        IsAuthenticated: boolean;
    }
}

declare module Orckestra.Composer {
    interface IValidationCallback {
        (): Q.Promise<boolean>;
    }
    interface IUpdateCallback {
        (): Q.Promise<any>;
    }
    interface IRegisterOptions {
        validationCallback: Function;
        updateCallback: Function;
    }
    interface IRegisterControlOptions extends IRegisterOptions {
        isReady: boolean;
    }
}

declare module Orckestra.Composer {
    class CheckoutService implements ICheckoutService {
        private static instance;
        static checkoutStep: number;
        private orderConfirmationCacheKey;
        private orderCacheKey;
        private window;
        private eventHub;
        private registeredControllers;
        private allControllersReady;
        private cacheProvider;
        protected cartService: ICartService;
        protected membershipService: IMembershipService;
        protected regionService: IRegionService;
        protected shippingMethodService: ShippingMethodService;
        static getInstance(): ICheckoutService;
        constructor();
        protected registerAllControllersInitialized(): void;
        private initialize();
        registerController(controller: IBaseCheckoutController): void;
        unregisterController(controllerName: string): void;
        renderControllers(checkoutContext: ICheckoutContext): Q.Promise<void[]>;
        updatePostalCode(postalCode: string): Q.Promise<void>;
        invalidateCache(): Q.Promise<void>;
        getCart(): Q.Promise<any>;
        updateCart(): Q.Promise<IUpdateCartResult>;
        completeCheckout(): Q.Promise<ICompleteCheckoutResult>;
        private buildCartUpdateViewModel(vm);
        private getCartValidation(vm);
        private getCartUpdateViewModel(vm);
        private collectValidationPromises();
        private collectUpdateModelPromises();
        private handleCheckoutSecurity(cart, targetedStep);
        private handleError(reason);
        setOrderConfirmationToCache(orderConfirmationviewModel: any): void;
        getOrderConfirmationFromCache(): Q.Promise<any>;
        clearOrderConfirmationFromCache(): void;
        setOrderToCache(orderConfirmationviewModel: any): void;
    }
}

declare module Orckestra.Composer {
    class BaseCheckoutController extends Orckestra.Composer.Controller implements Orckestra.Composer.IBaseCheckoutController {
        protected debounceHandle: any;
        protected debounceTimeout: number;
        protected formInstances: IParsley[];
        protected checkoutService: ICheckoutService;
        viewModelName: string;
        initialize(): void;
        protected registerController(): void;
        unregisterController(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<any>;
        getValidationPromise(): Q.Promise<boolean>;
        getUpdateModelPromise(): Q.Promise<any>;
        protected registerSubscriptions(): void;
        protected getViewModelUpdated(): string;
        protected isValidForUpdate(): boolean;
        protected onRenderDataFailed(reason: any): void;
        protected removeLoading(): void;
    }
}

declare module Orckestra.Composer {
    class GuestCustomerInfoCheckoutController extends Orckestra.Composer.BaseCheckoutController {
        initialize(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        protected renderAuthenticated(checkoutContext: ICheckoutContext): void;
        protected renderUnauthenticated(checkoutContext: ICheckoutContext): void;
    }
}

declare module Orckestra.Composer {
    class ShippingAddressCheckoutService {
        private cartService;
        private eventHub;
        constructor(cartService: ICartService, eventHub: IEventHub);
        setCheapestShippingMethodUsing(postalCode: string): Q.Promise<void>;
        private handleError(reason);
    }
}

declare module Orckestra.Composer {
    class ShippingAddressCheckoutController extends Orckestra.Composer.BaseCheckoutController {
        protected shippingAddressCheckoutService: ShippingAddressCheckoutService;
        initialize(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        private getRegionCode(cart);
        changePostalCode(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class ShippingMethodCheckoutController extends Orckestra.Composer.BaseCheckoutController {
        protected debounceMethodSelected: Function;
        initialize(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        methodSelected(actionContext: IControllerActionContext): void;
        private methodSelectedImpl(actionContext);
        private updateShippingProviderId(actionContext);
        protected handleError(reason: any): void;
    }
}

declare module Orckestra.Composer {
    class BillingAddressCheckoutController extends Orckestra.Composer.BaseCheckoutController {
        initialize(): void;
        changeUseShippingAddress(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        protected renderAuthenticated(checkoutContext: ICheckoutContext): void;
        protected renderUnauthenticated(checkoutContext: ICheckoutContext): void;
        protected registerSubscriptions(): void;
        protected renderDataFailed(reason: any): void;
        private getRegionCode(cart);
        private onRendered();
        private useShippingAddress();
        private onPostalCodeChanged(useShippingAddress, cart);
        private getVisibleForms();
        private setBillingAddressFormVisibility();
        private setBillingAddressFormValidation();
        private isBillingAddressFormValidationEnabled();
        private disableBillingAddressFormValidation();
        private isBillingAddressFormInstance(formInstance);
        private enableBillingAddressFormValidation();
    }
}

declare module Orckestra.Composer {
    interface ICustomerRepository {
        updateAccount(formData: any, returnUrl: string): Q.Promise<any>;
        getAddresses(): Q.Promise<any>;
        getRecurringCartAddresses(cartName: string): Q.Promise<any>;
        getRecurringTemplateAddresses(id: string): Q.Promise<any>;
        createAddress(formData: any, returnUrl: string): Q.Promise<any>;
        updateAddress(formData: any, addressId: string, returnUrl: string): Q.Promise<any>;
        deleteAddress(addressId: JQuery, returnUrl: string): Q.Promise<any>;
        setDefaultAddress(addressId: string, returnUrl: string): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class CustomerRepository implements ICustomerRepository {
        updateAccount(formData: any, returnUrl: string): Q.Promise<any>;
        getAddresses(): Q.Promise<any>;
        getRecurringCartAddresses(cartName: string): Q.Promise<any>;
        getRecurringTemplateAddresses(id: string): Q.Promise<any>;
        createAddress(formData: any, returnUrl: string): Q.Promise<any>;
        updateAddress(formData: any, addressId: string, returnUrl: string): Q.Promise<any>;
        deleteAddress(addressId: JQuery, returnUrl: string): Q.Promise<any>;
        setDefaultAddress(addressId: string, returnUrl: string): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface ICustomerService {
        updateAccount(formData: any, returnUrl: string): Q.Promise<any>;
        getAddresses(): Q.Promise<any>;
        getRecurringCartAddresses(cartName: string): Q.Promise<any>;
        getRecurringTemplateAddresses(id: string): Q.Promise<any>;
        createAddress(formData: any, returnUrl: string): Q.Promise<any>;
        updateAddress(formData: any, addressId: string, returnUrl: string): Q.Promise<any>;
        deleteAddress(addressId: JQuery, returnUrl: string): Q.Promise<any>;
        setDefaultAddress(addressId: string, returnUrl: string): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class CustomerService implements ICustomerService {
        private memoizeGetAdresses;
        protected customerRepository: ICustomerRepository;
        constructor(customerRepository: ICustomerRepository);
        updateAccount(formData: any, returnUrl: string): Q.Promise<any>;
        getAddresses(): Q.Promise<any>;
        private getAddressesImpl();
        getRecurringCartAddresses(cartName: string): Q.Promise<any>;
        private getRecurringCartAddressesImpl(cartName);
        getRecurringTemplateAddresses(id: string): Q.Promise<any>;
        private getRecurringTemplateAddressesImpl(id);
        createAddress(formData: any, returnUrl: string): Q.Promise<any>;
        updateAddress(formData: any, addressId: string, returnUrl: string): Q.Promise<any>;
        deleteAddress(addressId: JQuery, returnUrl: string): Q.Promise<any>;
        setDefaultAddress(addressId: string, returnUrl: string): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    enum MyAccountStatus {
        Success = 0,
        InvalidTicket = 1,
        DuplicateEmail = 2,
        DuplicateUserName = 3,
        InvalidQuestion = 4,
        InvalidPassword = 5,
        InvalidPasswordAnswer = 6,
        InvalidEmail = 7,
        Failed = 8,
        UserRejected = 9,
        RequiresApproval = 10,
        AjaxFailed = 11,
    }
}

interface AddressDto {
    Id: string;
    IsPreferredShipping: boolean;
    IsPreferredBilling: boolean;
}

declare module Orckestra.Composer {
    class BillingAddressRegisteredCheckoutService {
        protected customerService: ICustomerService;
        constructor(customerService: ICustomerService);
        getBillingAddresses(cart: any): Q.Promise<any>;
        getSelectedBillingAddressId(cart: any, addressList: any): string;
        private isBillingAddressFromCartValid(cart, addressList);
        private getPreferredBillingAddressId(addressList);
    }
}

declare module Orckestra.Composer {
    class UIModal {
        private modalContext;
        private confirmDeferred;
        private confirmAction;
        private window;
        private sender;
        private modalContextSelector;
        private container;
        constructor(window: Window, modalContextSelector: string, confirmAction: any, sender: any, container?: any);
        private registerDomEvents(container);
        private unregisterDomEvents();
        openModal: (event: JQueryEventObject) => void;
        confirmModal(): void;
        cancelModal(): void;
        dispose(): void;
    }
}

declare module Orckestra.Composer {
    class BillingAddressRegisteredCheckoutController extends Orckestra.Composer.BaseCheckoutController {
        protected debounceChangeBillingMethod: Function;
        protected modalElementSelector: string;
        private uiModal;
        protected customerService: ICustomerService;
        protected billingAddressRegisteredCheckoutService: BillingAddressRegisteredCheckoutService;
        initialize(): void;
        protected registerSubscriptions(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        protected renderUnauthenticated(checkoutContext: ICheckoutContext): void;
        protected renderAuthenticated(checkoutContext: ICheckoutContext): Q.Promise<any>;
        protected onRendered(e: IEventInformation): void;
        protected setSelectedBillingAddress(): void;
        changeBillingAddress(actionContext: IControllerActionContext): void;
        protected changeBillingAddressImpl(): void;
        protected deleteAddress(event: JQueryEventObject): Q.Promise<void>;
        deleteAddressConfirm(actionContext: IControllerActionContext): void;
        private onAddressDeleted(e);
        private useShippingAddress();
        private getVisibleForms();
        changeUseShippingAddress(): void;
        private setBillingAddressFormVisibility();
        private setBillingAddressFormValidation();
        private isBillingAddressFormValidationEnabled();
        private disableBillingAddressFormValidation();
        private isBillingAddressFormInstance(formInstance);
        private enableBillingAddressFormValidation();
        private renderFailedForm(status);
        protected handleError(reason: any): void;
    }
}

declare module Orckestra.Composer {
    class CheckoutOrderConfirmationController extends Orckestra.Composer.Controller {
        private cacheProvider;
        private orderConfirmationCacheKey;
        private orderCacheKey;
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    class CheckoutCompleteController extends Orckestra.Composer.BaseCheckoutController {
        initialize(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    class MyAccountController extends Orckestra.Composer.Controller {
        initialize(): void;
        protected getFormData(actionContext: IControllerActionContext): any;
        protected renderFormErrorMessages(reason: any): void;
    }
}

declare module Orckestra.Composer {
    class AddressListController extends Orckestra.Composer.MyAccountController {
        private deleteModalElementSelector;
        private uiModal;
        protected customerService: ICustomerService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onAddressDeleted(e);
        setDefaultAddress(actionContext: IControllerActionContext): void;
        deleteAddress(event: JQueryEventObject): void;
        private onDeleteAddressFulfilled(result, $addressListItem);
        deleteAddressConfirm(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class ChangePasswordController extends Orckestra.Composer.MyAccountController {
        protected membershipService: IMembershipService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onPasswordChanged(e);
        changePassword(actionContext: IControllerActionContext): void;
        private onChangePasswordFulfilled(result);
    }
}

declare module Orckestra.Composer {
    var urlHelper: {
        getURLParameter: (url: any, name: any) => string;
    };
}

declare module Orckestra.Composer {
    class CreateAccountController extends Orckestra.Composer.MyAccountController {
        protected membershipService: IMembershipService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onAccountCreated(e);
        createAccount(actionContext: IControllerActionContext): void;
        private onRegisterFulfilled(result);
    }
}

declare module Orckestra.Composer {
    class EditAddressController extends Orckestra.Composer.MyAccountController {
        protected _formInstances: IParsley[];
        protected customerService: ICustomerService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onAddressCreatedOrUpdated(e);
        private rebuildRegionSelector(regions);
        adjustPostalCode(actionContext: IControllerActionContext): void;
        createAddress(actionContext: IControllerActionContext): void;
        private onCreateAddressFulfilled(result);
        updateAddress(actionContext: IControllerActionContext): void;
        private onUpdateAddressFulfilled(result);
    }
}

declare module Orckestra.Composer {
    class ForgotPasswordController extends Orckestra.Composer.MyAccountController {
        protected membershipService: IMembershipService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onForgotPasswordInstructionSent(e);
        forgotPassword(actionContext: IControllerActionContext): void;
        private onForgotPasswordFulfilled(result);
    }
}

declare module Orckestra.Composer {
    class AccountHeaderController extends Orckestra.Composer.Controller {
        protected membershipService: IMembershipService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onLoggedOut(data);
        fullLogout(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    interface ISignInHeaderRepository {
        getSignInHeader(param: any): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class SignInHeaderRepository implements Orckestra.Composer.ISignInHeaderRepository {
        getSignInHeader(param: any): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class SignInHeaderService {
        private cacheKey;
        private cachePolicy;
        private cacheProvider;
        private signInHeaderRepository;
        constructor(signInHeaderRepository: ISignInHeaderRepository);
        getSignInHeader(param: any): Q.Promise<any>;
        private canHandle(reason);
        getFreshSignInHeader(param: any): Q.Promise<any>;
        buildSignedInCacheKey(param: any): string;
        invalidateCache(): Q.Promise<void>;
        private getSignInHeaderFromCache(param);
        private setSignInHeaderToCache(param, cart);
    }
}

declare module Orckestra.Composer {
    class UpdateAccountController extends Orckestra.Composer.MyAccountController {
        protected customerService: ICustomerService;
        protected signInHeaderService: SignInHeaderService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onAccountUpdated(e);
        enableSubmitButton(actionContext: IControllerActionContext): void;
        updateAccount(actionContext: IControllerActionContext): void;
        protected onUpdateAccountFulfilled(result: any): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class NewPasswordController extends Orckestra.Composer.MyAccountController {
        protected membershipService: IMembershipService;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onPasswordChanged(e);
        newPassword(actionContext: IControllerActionContext): void;
        private onResetPasswordFulfilled(result);
    }
}

declare module Orckestra.Composer {
    class ReturningCustomerController extends Orckestra.Composer.Controller {
        protected membershipService: IMembershipService;
        protected cacheProvider: ICacheProvider;
        initialize(): void;
        protected registerSubscriptions(): void;
        private onLoggedIn(data);
        login(actionContext: IControllerActionContext): void;
        private loginImpl(actionContext);
        private onLoginFulfilled(result, busy);
        private onLoginRejected(reason, busy);
        private renderFailedForm(status);
    }
}

declare module Orckestra.Composer {
    class WishListController extends Orckestra.Composer.Controller {
        protected _wishListService: IWishListService;
        protected _cartService: CartService;
        initialize(): void;
        addToCart(actionContext: IControllerActionContext): void;
        protected getListNameForAnalytics(): string;
        protected getProductDataForAnalytics(productId: any, variant: any, displayName: any, price: any, brand: any, category: any): any;
    }
}

declare module Orckestra.Composer {
    class MyWishListController extends Orckestra.Composer.WishListController {
        initialize(): void;
        protected registerSubscriptions(): void;
        copyShareUrl(actionContext: IControllerActionContext): any;
        deleteLineItem(actionContext: IControllerActionContext): void;
        protected onWishListUpdated(e: IEventInformation): void;
        protected renderWishListQuantity(wishList: any): void;
        protected getListNameForAnalytics(): string;
    }
}

declare module Orckestra.Composer {
    class SharedWishListController extends Orckestra.Composer.WishListController {
        initialize(): void;
        protected getListNameForAnalytics(): string;
    }
}

declare module Orckestra.Composer {
    class WishListInHeaderController extends Orckestra.Composer.Controller {
        private _wishListService;
        initialize(): void;
        private initializeWishListQuantity();
        protected registerSubscriptions(): void;
        protected onWishListUpdated(e: IEventInformation): void;
        protected onRefreshUser(e: IEventInformation): Q.Promise<any>;
        protected renderWishList(wishList: any): void;
        protected onError(reason: any): void;
    }
}

declare module Orckestra.Composer {
    class ShippingAddressRegisteredService {
        protected customerService: ICustomerService;
        constructor(customerService: ICustomerService);
        getShippingAddresses(cart: any): Q.Promise<any>;
        getSelectedShippingAddressId(cart: any, addressList: any): string;
        private isShippingAddressFromCartValid(cart, addressList);
        private getPreferredShippingAddressId(addressList);
    }
}

declare module Orckestra.Composer {
    class ShippingAddressRegisteredController extends Orckestra.Composer.BaseCheckoutController {
        private uiModal;
        protected debounceChangeShippingMethod: Function;
        protected deleteModalElementSelector: string;
        protected customerService: ICustomerService;
        protected shippingAddressRegisteredService: ShippingAddressRegisteredService;
        initialize(): void;
        protected registerSubscriptions(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        private onRendered(e);
        private onAddressDeleted(e);
        changeShippingAddress(actionContext: IControllerActionContext): void;
        private changeShippingAddressImpl();
        deleteAddressConfirm(actionContext: IControllerActionContext): void;
        private deleteAddress(event);
        private renderFailedForm(status);
        private handleError(reason);
    }
}

declare module Orckestra.Composer {
    class CheckoutOrderSummaryController extends Orckestra.Composer.BaseCheckoutController {
        protected recurringOrderService: IRecurringOrderService;
        initialize(): void;
        protected registerSubscriptions(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        private reRender();
        private renderLoading();
        nextStep(actionContext: IControllerActionContext): void;
        handleRecurringOrderCheckoutSecurity(checkoutContext: ICheckoutContext): any;
    }
}

declare module Orckestra.Composer {
    class CompleteCheckoutOrderSummaryController extends Orckestra.Composer.BaseCheckoutController {
        initialize(): void;
        renderData(checkoutContext: ICheckoutContext): Q.Promise<void>;
        nextStep(actionContext: IControllerActionContext): void;
        protected registerSubscriptions(): void;
        private renderLoading();
        private reRender();
    }
}

declare module Orckestra.Composer {
    interface IPaymentService {
        getPaymentMethods(providers: Array<string>): Q.Promise<IPaymentViewModel>;
        getActivePayment(): Q.Promise<IActivePaymentViewModel>;
        removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void>;
        setPaymentMethod(request: any): Q.Promise<IPaymentViewModel>;
    }
}

declare module Orckestra.Composer {
    interface IActivePaymentViewModel {
        Id: string;
        PaymentMethodType: string;
        ShouldCapturePayment: boolean;
        CapturePaymentUrl: string;
        PaymentStatus: string;
        ProviderType: string;
        ProviderName: string;
        CanSavePaymentMethod: boolean;
    }
}

declare module Orckestra.Composer {
    class BaseCheckoutPaymentProvider implements Orckestra.Composer.IDisposable {
        protected _providerType: string;
        protected _providerName: string;
        protected _eventHub: IEventHub;
        private _window;
        constructor(window: Window, eventHub: IEventHub, providerType: string, providerName: string);
        providerType: string;
        providerName: string;
        protected window: Window;
        validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean>;
        submitPayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<any>;
        protected getForm(): JQuery;
        dispose(): void;
    }
}

declare module Orckestra.Composer {
    interface IPaymentMethodViewModel {
        Id: string;
        PaymentProviderName: string;
        DisplayName: string;
        IsSelected: boolean;
        JsonContext: any;
        PaymentType: string;
        Default: boolean;
        IsValid: boolean;
    }
}

declare module Orckestra.Composer {
    interface IPaymentRepository {
        getPaymentMethods(providers: Array<string>): Q.Promise<IPaymentViewModel>;
        getActivePayment(): Q.Promise<IActivePaymentViewModel>;
        removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void>;
        setPaymentMethod(request: any): Q.Promise<IPaymentViewModel>;
    }
}

declare module Orckestra.Composer {
    class PaymentService implements IPaymentService {
        private eventHub;
        private paymentRepository;
        constructor(eventHub: IEventHub, paymentRepository: IPaymentRepository);
        getPaymentMethods(providers: Array<string>): Q.Promise<IPaymentViewModel>;
        getActivePayment(): Q.Promise<IActivePaymentViewModel>;
        removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void>;
        setPaymentMethod(request: any): Q.Promise<IPaymentViewModel>;
    }
}

declare module Orckestra.Composer {
    interface IPaymentViewModel {
        PaymentId: string;
        PaymentMethods: Array<IPaymentMethodViewModel>;
        UponReceptionPaymentMethodViewModels: Array<IPaymentMethodViewModel>;
        OnlinePaymentMethodViewModels: Array<IPaymentMethodViewModel>;
        CreditCardPaymentMethod: IPaymentMethodViewModel;
        SavedCreditCards: Array<IPaymentMethodViewModel>;
        IsSavedCreditCardSelected: boolean;
        IsLoading: boolean;
        IsProviderLoading: boolean;
        ActivePaymentViewModel: IActivePaymentViewModel;
    }
}

declare module Orckestra.Composer {
    interface IPaymentProfileListItemViewModel {
        ScopeId: string;
        ProviderName: string;
        PaymentProfileId: string;
        ExternalIds: string;
    }
}

declare module Orckestra.Composer {
    interface IPaymentProfileListViewModel {
        PaymentProfiles: Array<IPaymentProfileListItemViewModel>;
    }
}

declare module Orckestra.Composer {
    class PaymentRepository implements IPaymentRepository {
        getPaymentMethods(providers: Array<string>): Q.Promise<IPaymentViewModel>;
        getActivePayment(): Q.Promise<IActivePaymentViewModel>;
        removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void>;
        setPaymentMethod(request: any): Q.Promise<IPaymentViewModel>;
    }
}

declare module Orckestra.Composer {
    class CheckoutPaymentProviderFactory {
        private _eventHub;
        private _window;
        constructor(window: Window, eventHub: IEventHub);
        hasProvider(providerType: string): boolean;
        getInstance(providerType: string, providerName: string): BaseCheckoutPaymentProvider;
    }
}

declare module Orckestra.Composer {
    class CheckoutPaymentController extends Orckestra.Composer.BaseCheckoutController {
        protected paymentProviders: Array<BaseCheckoutPaymentProvider>;
        protected activePaymentProvider: BaseCheckoutPaymentProvider;
        protected debounceChangePaymentMethod: (args: {
            activeMethodId: string;
            paymentProviderName: string;
            paymentType: string;
        }) => void;
        paymentService: IPaymentService;
        protected viewModel: IPaymentViewModel;
        protected _window: Window;
        protected _busyHandler: UIBusyHandle;
        initialize(): void;
        selectCreditCardPaymentMethod(): void;
        payWithSavedCreditCard(): void;
        payWithCreditCard(): void;
        changePaymentMethodInternal(activeMethodId: string, paymentProviderName: string, paymentType?: string): void;
        changePaymentMethod(actionContext: IControllerActionContext): void;
        protected executeChangePaymentMethod(args: {
            activeMethodId: string;
            paymentProviderName: string;
            paymentType: string;
        }): void;
        renderData(): Q.Promise<void>;
        getValidationPromise(): Q.Promise<boolean>;
        getUpdateModelPromise(): Q.Promise<any>;
        dispose(): void;
        protected findDefaultPaymentMethod(paymentMethods: Array<IPaymentMethodViewModel>): IPaymentMethodViewModel;
        protected onInitializePaymentMethodFailed(reason: any): void;
        protected renderView(): void;
        protected setPaymentMethod(paymentId: string, activeMethodId: string, paymentProviderName: string, paymentType: string): Q.Promise<any>;
        protected onChangePaymentMethodFailed(reason: any): void;
        protected findPaymentProviderByType(providerType: string): BaseCheckoutPaymentProvider;
        protected getPaymentProviders(): Array<BaseCheckoutPaymentProvider>;
        protected releaseBusyHandler(): void;
    }
}

declare module Orckestra.Composer {
    class CheckoutNavigationController extends Orckestra.Composer.Controller {
        protected currentStep: any;
        protected viewModelName: string;
        protected checkoutService: ICheckoutService;
        initialize(): void;
        renderData(): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    class SignInHeaderController extends Orckestra.Composer.Controller {
        protected signInHeaderService: SignInHeaderService;
        initialize(): void;
        private initializeSignInHeader();
        protected registerSubscriptions(): void;
        protected onLoggedOut(e: IEventInformation): Q.Promise<any>;
        protected onLoggedIn(e: IEventInformation): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface IGetOrderParameters {
        page: number;
    }
}

declare module Orckestra.Composer {
    class OrderService {
        getPastOrders(options?: IGetOrderParameters): Q.Promise<any>;
        getCurrentOrders(options?: IGetOrderParameters): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class CurrentOrdersController extends Controller {
        protected orderService: OrderService;
        initialize(): void;
        getOrders(context: IControllerActionContext): void;
        private getCurrentOrders(param?);
    }
}

declare module Orckestra.Composer {
    class PastOrdersController extends Controller {
        protected orderService: OrderService;
        initialize(): void;
        getOrders(context: IControllerActionContext): void;
        private getPastOrders(param?);
    }
}

declare module Orckestra.Composer {
    class OrderDetailsController extends Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    interface IGetOrderDetailsUrlRequest {
        OrderNumber: string;
        Email: string;
    }
}

declare module Orckestra.Composer {
    interface IGuestOrderDetailsViewModel {
        Url: string;
    }
}

declare module Orckestra.Composer {
    interface IFindOrderService {
        getOrderDetailsUrl(req: IGetOrderDetailsUrlRequest): Q.Promise<IGuestOrderDetailsViewModel>;
    }
}

declare module Orckestra.Composer {
    class FindOrderService implements IFindOrderService {
        private eventHub;
        constructor(eventHub: IEventHub);
        getOrderDetailsUrl(req: IGetOrderDetailsUrlRequest): Q.Promise<IGuestOrderDetailsViewModel>;
    }
}

declare module Orckestra.Composer {
    interface IFindMyOrderViewModel {
        OrderNumber: string;
        Email: string;
        OrderNotFound: boolean;
    }
}

declare module Orckestra.Composer {
    class FindMyOrderController extends Orckestra.Composer.Controller {
        private findOrderService;
        initialize(): void;
        getWindow(): Window;
        onFindMyOrder(actionContext: IControllerActionContext): void;
        private findOrderAsync(request);
        private handleOrderNotFound(reason, request);
    }
}

declare module Orckestra.Composer {
    class ErrorController extends Controller {
        private lastErrorCodes;
        initialize(): void;
        protected subscribeToEvents(): void;
        protected handleGeneralError(errors: IErrorCollection, source: string): void;
        protected scrollToElement(element: JQuery, offsetDiff?: number): void;
    }
}

declare module Orckestra.Composer {
    interface IStoreLocatorService {
        getMapConfiguration(): Q.Promise<any>;
        getStore(storeNumber: string): Q.Promise<any>;
        getStores(southWest: google.maps.LatLng, northEast: google.maps.LatLng, searchPoint: google.maps.LatLng, page: number, pageSize: number): Q.Promise<any>;
        getMarkers(southWest: google.maps.LatLng, northEast: google.maps.LatLng, zoomLevel: number, searchPoint: google.maps.LatLng, isSearch: boolean, pageSize: number): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class StoreLocatorEndPointUrls {
        static GetStoresEndPointUrl: string;
        static GetStoreEndPointUrl: string;
        static GetMapConfigurationEndPointUrl: string;
        static GetMarkersEndPointUrl: string;
    }
}

declare module Orckestra.Composer {
    class StoreLocatorService implements IStoreLocatorService {
        private memoizeStore;
        getStore(storeNumber: string): Q.Promise<any>;
        private getStoreImpl(storeNumber);
        getStores(southWest: google.maps.LatLng, northEast: google.maps.LatLng, searchPoint: google.maps.LatLng, page: number, pageSize: number): Q.Promise<any>;
        getMapConfiguration(): Q.Promise<any>;
        getMarkers(southWest: google.maps.LatLng, northEast: google.maps.LatLng, zoomLevel: number, searchPoint: google.maps.LatLng, isSearch: boolean, pageSize: number): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface IMapOptions {
        mapCanvas: HTMLElement;
        options: google.maps.MapOptions;
        infoWindowMaxWidth?: number;
    }
}

declare module Orckestra.Composer {
    class Marker {
        private _key;
        private _value;
        private _storeNumber;
        private _isCluster;
        constructor(marker: MarkerWithLabel);
        key: any;
        value: MarkerWithLabel;
        storeNumber: string;
        isCluster: boolean;
        setMap(map: any): void;
        setPosition(position: google.maps.LatLng): void;
    }
}

declare module Orckestra.Composer {
    class MarkerPool {
        private markers;
        private indexedMarkersByKey;
        private _map;
        private _onMarkerCreate;
        constructor(map: google.maps.Map, onMarkerCreate: any);
        getMarkers(): any;
        get(isCluster?: boolean): Marker;
        getExisting(key: any): any;
        index(marker: Marker): void;
        hasClusters(): boolean;
        protected createMarker(): Marker;
        protected createClusterMarker(): Marker;
        releaseAll(): void;
        releaseByIndex(index: string): void;
        releaseClusters(): void;
        releaseMarkersByIds(iscluster: boolean, id: string): void;
    }
}

declare module Orckestra.Composer {
    class MapService {
        private eventHub;
        private _map;
        private _markerPool;
        private _prevZoom;
        private _currentLocationMarker;
        private _informationWindow;
        private _projectionOverlay;
        private _mapInitialized;
        private _mapIdle;
        private _mapDragEnded;
        constructor(eventHub: IEventHub);
        initialize(mapOptions: IMapOptions): void;
        private setProjectionOverlay();
        getMap(): google.maps.Map;
        getInformationWindow(): google.maps.InfoWindow;
        getBounds(markerPadding?: number): google.maps.LatLngBounds;
        getZoom(): number;
        onNewMarkerCreated(marker: Marker): void;
        mapInitialized(): Q.Promise<MapService>;
        mapIdle(): Q.Promise<MapService>;
        mapDragEnded(): Q.Promise<MapService>;
        centerMap(storeBounds: any): void;
        openInformationWindow(content: any, anchor?: any): void;
        setLocationInMap(point: google.maps.LatLng, zoomLevel?: number): void;
        extendBounds(point1: google.maps.LatLng, point2: google.maps.LatLng): void;
        createMarkerOnMap(location: google.maps.LatLng, title: string): google.maps.Marker;
        setMarkers(markerInfos: any[], isSearch?: boolean): void;
        private transformResult(result, markerPool, action);
    }
}

declare module Orckestra.Composer {
    class GeoLocationService {
        private _browserGeolocation;
        private _geocoder;
        private _currenctLocation;
        constructor();
        geolocate(): Q.Promise<any>;
        getCurrentLocation(): Q.Promise<google.maps.LatLng>;
        getAddtressByLocation(location: google.maps.LatLng): Q.Promise<string>;
        getLocationByAddress(address: string): Q.Promise<google.maps.LatLng>;
        updateDirectionLinksWithLatLngSourceAddress(container: JQuery, sourceLocation: google.maps.LatLng): void;
        getDirectionLatLngSourceAddress(baseUrl: string, sourceLocation: google.maps.LatLng): string;
    }
}

declare module Orckestra.Composer {
    interface IStoreLocatorInitializationOptions {
        mapId: string;
        coordinates: {
            Lat: number;
            Lng: number;
        };
        showNearestStoreInfo: boolean;
        zoomLevel?: number;
        markerPadding?: number;
    }
}

declare module Orckestra.Composer {
    class StoreLocatorHistoryState {
        point: google.maps.LatLng;
        page: number;
        zoom: number;
        center: google.maps.LatLng;
        pos: number;
    }
}

declare module Orckestra.Composer {
    class StoreLocatorController extends Controller {
        protected _storeLocatorService: IStoreLocatorService;
        protected _geoService: GeoLocationService;
        protected _mapService: MapService;
        protected _storeLocatorOptions: IStoreLocatorInitializationOptions;
        protected _historyState: StoreLocatorHistoryState;
        protected _isRestoreListPaging: boolean;
        protected _searchPointAddressCacheKey: string;
        protected cache: ICache;
        private _searchBox;
        private _searchBoxJQ;
        private _searchPoint;
        private _searchPointMarker;
        private _isSearch;
        private _timer;
        private _enterPressedTimer;
        private _getCurrentLocation;
        getCurrentLocation(): Q.Promise<google.maps.LatLng>;
        initialize(options?: IStoreLocatorInitializationOptions): void;
        private registerSubscriptions();
        private initSearchBox();
        private searchBoxOnPlacesChanged();
        private searchBoxOnEnterPressed();
        protected getMapOptions(): IMapOptions;
        private searchBoxSetBounds(bounds);
        protected onMapBoundsUpdated(data?: any, isSearch?: boolean): void;
        protected onMarkerClick(marker?: Marker): void;
        protected onClusterClick(marker?: Marker): void;
        protected updateMarkers(data?: any, isSearch?: boolean): void;
        private setSearchLocationInMap(point, zoomLevel?);
        private createSearchPoitMarker();
        currentLocationAction(actionContext: IControllerActionContext): void;
        nextPage(actionContext: IControllerActionContext): void;
        rememberPosition(actionContext: IControllerActionContext): void;
        protected setNearestStoreInfo(info: string): void;
        protected getStoresForPage(page: number, pageSize?: number, element?: any): void;
        protected renderStoresList(stores: any, target: HTMLElement): void;
        protected setGoogleDirectionLinks(): Q.Promise<any>;
        protected historyPushState(page: number, point?: google.maps.LatLng, zoom?: number, center?: google.maps.LatLng, elementPos?: number): void;
        protected parseHistoryState(): void;
        protected restoreMapFromHistoryState(): void;
        protected handlePromiseFail(title: string, reason: any): void;
    }
}

declare module Orckestra.Composer {
    class StoreDetailsController extends Controller {
        protected _geoService: GeoLocationService;
        private _map;
        private _marker;
        initialize(): void;
        protected setGoogleDirectionLink(): void;
    }
}

declare module Orckestra.Composer {
    class StoresDirectoryController extends Controller {
        protected _geoService: GeoLocationService;
        private _searchBox;
        initialize(): void;
        protected initializeSearchBox(): void;
        currentLocationAction(actionContext: IControllerActionContext): void;
        protected setGoogleDirectionLinks(): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    class GetStoresInventoryParam {
        Sku: string;
        SearchPoint: google.maps.LatLng;
        Page: number;
        Pagesize: number;
    }
}

declare module Orckestra.Composer {
    interface IStoreInventoryService {
        getStoresInventory(param: GetStoresInventoryParam): Q.Promise<any>;
        getDefaultAddress(): Q.Promise<any>;
        getSkuSelection(productId: string): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class StoreInventoryService implements IStoreInventoryService {
        getStoresInventory(param: GetStoresInventoryParam): Q.Promise<any>;
        getDefaultAddress(): Q.Promise<any>;
        getSkuSelection(productId: string): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class StoreInventoryController extends Controller {
        protected _concern: string;
        protected _service: IStoreInventoryService;
        protected _geoService: GeoLocationService;
        protected _searchPointAddressCacheKey: string;
        protected cache: ICache;
        private _searchBox;
        private _searchBoxJQ;
        private _searchPoint;
        private _selectedSku;
        private _isAuthenticated;
        private _pageSize;
        private _productId;
        private _getCurrentLocation;
        getCurrentLocation(): Q.Promise<google.maps.LatLng>;
        initialize(): void;
        private registerSubscriptions();
        private getDataFromContextViewModel();
        protected initSearchBox(): void;
        protected searchPointChanged(e: IEventInformation): void;
        protected onSelectedVariantIdChanged(e: IEventInformation): void;
        protected onHashChanged(): void;
        protected getStoresInventory(): Q.Promise<any>;
        protected nextPage(actionContext: IControllerActionContext): void;
        protected setGoogleDirectionLinks(): Q.Promise<any>;
        protected getStoresInventoryParam(page?: number): GetStoresInventoryParam;
        protected getDefaultAddress(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class RecurringScheduleController extends Orckestra.Composer.Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    class MyRecurringScheduleController extends Orckestra.Composer.RecurringScheduleController {
        protected recurringOrderService: IRecurringOrderService;
        protected debounceUpdateLineItem: (args: any) => void;
        protected updateWaitTime: number;
        protected busyHandler: UIBusyHandle;
        protected viewModelName: string;
        protected uiModalConfirmRemove: UIModal;
        protected modalElementSelectorRemove: string;
        protected window: Window;
        initialize(): void;
        updateLineItemQuantity(actionContext: Orckestra.Composer.IControllerActionContext): void;
        applyUpdateLineItemQuantity(args: any): void;
        protected onLineItemQuantityFailed(context: JQuery, reason: any): void;
        updateQuantity(action: string, quantity: number): number;
        protected releaseBusyHandler(): void;
        reRenderPage(vm: any): void;
        deleteLineItemConfirm(actionContext: IControllerActionContext): void;
        deleteLineItem(event: JQueryEventObject): Q.Promise<void>;
        editDetailsClick(actionContext: IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class RecurringScheduleDetailsController extends Orckestra.Composer.Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    class DatepickerService {
        static renderDatepicker(elementId: string, minDate?: Date, language?: string): void;
    }
}

declare module Orckestra.Composer {
    class MyRecurringScheduleDetailsController extends Orckestra.Composer.RecurringScheduleDetailsController {
        protected recurringOrderService: IRecurringOrderService;
        protected viewModelName: string;
        protected id: string;
        protected viewModel: any;
        protected modalElementSelector: string;
        protected uiModal: UIModal;
        protected busyHandler: UIBusyHandle;
        protected window: Window;
        protected customerService: ICustomerService;
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService;
        initialize(): void;
        getRecurringTemplateDetail(): void;
        getAvailableEditList(): void;
        reRenderPage(vm: any): void;
        renderShippingMethods(vm: any): void;
        renderAddresses(vm: any): void;
        renderPayment(vm: any): void;
        getAddresses(): void;
        getShippingMethodsList(): void;
        getShippingMethods(): Q.Promise<any>;
        getPaymentMethods(): void;
        useShippingAddress(): Boolean;
        changeUseShippingAddress(): void;
        setBillingAddressFormVisibility(): void;
        protected setSelectedBillingAddress(): void;
        deleteAddressConfirm(actionContext: IControllerActionContext): void;
        protected deleteAddress(event: JQueryEventObject): Q.Promise<void>;
        protected releaseBusyHandler(): void;
        saveRecurringOrderTemplate(actionContext: IControllerActionContext): Q.Promise<void>;
        nextOcurrenceIsValid(value: any): boolean;
        convertDateToUTC(date: any): Date;
    }
}

declare module Orckestra.Composer {
    class RecurringCartsController extends Orckestra.Composer.Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    class MyRecurringCartsController extends Orckestra.Composer.RecurringCartsController {
        protected recurringOrderService: IRecurringOrderService;
        initialize(): void;
        getUpcomingOrders(): void;
    }
}

declare module Orckestra.Composer {
    class RecurringCartDetailsController extends Orckestra.Composer.Controller {
        initialize(): void;
    }
}

declare module Orckestra.Composer {
    class RecurringCartAddressRegisteredService {
        protected customerService: ICustomerService;
        constructor(customerService: ICustomerService);
        getRecurringCartAddresses(cart: any): Q.Promise<any>;
        getRecurringTemplateAddresses(id: any): Q.Promise<any>;
        getSelectedBillingAddressId(cart: any, addressList: any): string;
        isBillingAddressFromCartValid(cart: any, addressList: any): boolean;
        getPreferredBillingAddressId(addressList: any): string;
        getSelectedShippingAddressId(cart: any, addressList: any): string;
        isShippingAddressFromCartValid(cart: any, addressList: any): boolean;
        getPreferredShippingAddressId(addressList: any): string;
    }
}

declare module Orckestra.Composer {
    interface IStoreService {
        getStores(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class StoreService implements IStoreService {
        private static _instance;
        static instance(): StoreService;
        getStores(): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface ICreateVaultTokenOptions {
        CardHolderName: string;
        VaultTokenId: string;
        PaymentId: string;
        CreatePaymentProfile: boolean;
        PaymentProviderName: string;
    }
}

declare module Orckestra.Composer {
    interface ISetDefaultCustomerPaymentMethodViewModel {
        PaymentProviderName: string;
        PaymentMethodId: string;
    }
}

declare module Orckestra.Composer {
    interface IMonerisAddVaultProfileViewModel {
        Success: boolean;
        ErrorCode: string;
        ErrorMessage: string;
        ActivePaymentViewModel: IActivePaymentViewModel;
    }
}

declare module Orckestra.Composer {
    class MonerisPaymentService {
        addCreditCard(request: ICreateVaultTokenOptions): Q.Promise<IMonerisAddVaultProfileViewModel>;
        setDefaultCustomerPaymentMethod(request: ISetDefaultCustomerPaymentMethodViewModel): Q.Promise<IPaymentMethodViewModel>;
        removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void>;
        removeRecurringCartPaymentMethod(paymentMethodId: string, paymentProviderName: string, cartName: string): Q.Promise<void>;
    }
}

declare module Orckestra.Composer {
    enum EditSection {
        NextOccurence = 0,
        ShippingMethod = 1,
        Address = 2,
        Payment = 3,
    }
    enum ShippingMethodType {
        Unspecified = 0,
        PickUp = 1,
        Delivery = 2,
        Shipping = 3,
        ShipToStore = 4,
    }
    class MyRecurringCartDetailsController extends Orckestra.Composer.RecurringCartDetailsController {
        protected recurringOrderService: IRecurringOrderService;
        protected storeService: IStoreService;
        protected paymentService: MonerisPaymentService;
        protected editNextOcurrence: boolean;
        protected editShippingMethod: boolean;
        protected editAddress: boolean;
        protected editPayment: boolean;
        protected originalShippingMethodType: string;
        protected hasShippingMethodTypeChanged: boolean;
        protected newShippingMethodType: any;
        protected viewModelName: string;
        protected viewModel: any;
        protected updateWaitTime: number;
        protected modalElementSelector: string;
        protected uiModal: UIModal;
        protected busyHandler: UIBusyHandle;
        protected window: Window;
        protected debounceUpdateLineItem: (args: any) => void;
        protected customerService: ICustomerService;
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService;
        initialize(): void;
        protected registerSubscriptions(): void;
        getRecurringCart(): void;
        toggleEditNextOccurence(actionContext: IControllerActionContext): void;
        saveEditNextOccurence(actionContext: IControllerActionContext): void;
        nextOcurrenceIsValid(value: any): boolean;
        convertDateToUTC(date: any): Date;
        toggleEditShippingMethod(actionContext: IControllerActionContext): void;
        closeOtherEditSections(actionContext: IControllerActionContext, type: EditSection): void;
        resetEditToggleFlags(): void;
        getShippingMethods(cartName: any): Q.Promise<any>;
        saveEditShippingMethod(actionContext: IControllerActionContext): void;
        methodSelected(actionContext: IControllerActionContext): void;
        manageSaveShippingMethod(newType: any, actionContext: any): void;
        reRenderCartPage(vm: any): void;
        toggleEditAddress(actionContext: IControllerActionContext): void;
        saveEditAddress(actionContext: IControllerActionContext): void;
        useShippingAddress(): Boolean;
        changeUseShippingAddress(): void;
        setBillingAddressFormVisibility(): void;
        protected setSelectedBillingAddress(): void;
        toggleEditPayment(actionContext: IControllerActionContext): void;
        updateLineItem(actionContext: IControllerActionContext): void;
        applyUpdateLineItemQuantity(args: any): void;
        protected onLineItemQuantityFailed(context: JQuery, reason: any): void;
        updateQuantity(action: string, quantity: number): number;
        deleteLineItem(actionContext: IControllerActionContext): void;
        protected onLineItemDeleteFailed(context: JQuery, reason: any): void;
        deleteAddressConfirm(actionContext: IControllerActionContext): void;
        protected deleteAddress(event: JQueryEventObject): Q.Promise<void>;
        onAddressDeleted(e: IEventInformation): void;
        saveEditPayment(actionContext: IControllerActionContext): void;
        protected releaseBusyHandler(): void;
        showError(errorCode: string, parentSelector: string): void;
    }
}


declare module Orckestra.Composer {
    interface IControllerActionSignature {
        (options: Composer.IControllerActionContext): void;
    }
}

declare module Orckestra.Composer {
    class AntiIFrameClickJackingPlugin implements IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
        private getOrigin(window);
    }
}

declare module Orckestra.Composer {
    class ComposerValidationLocalizationPlugin implements IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
        protected defineValidators(parsleyValidator: any): void;
    }
}

declare module Orckestra.Composer {
    class FocusElementPlugin implements IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
    }
}

declare module Orckestra.Composer {
    class HelpBubblesPlugin implements IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
    }
}

declare module Orckestra.Composer {
    class StickyAffixPlugin implements IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
    }
}


declare module Orckestra.Composer {
    interface ICheckoutGetCartPromiseFailureHandler {
        (error: any): void;
    }
}

declare module Orckestra.Composer {
    interface ICheckoutGetCartPromiseParam {
        targetedStep: number;
        forceGet?: boolean;
    }
}

declare module Orckestra.Composer {
    interface IGetPaymentMethodsOptions {
        Providers: string[];
    }
}

declare module Orckestra.Composer {
    interface IUpdatePaymentOptions {
        PaymentId: string;
        PaymentProviderName: string;
        PaymentMethodId: string;
    }
}

declare module Orckestra.Composer {
    class PaymentProvider {
        protected window: Window;
        protected eventHub: IEventHub;
        protected _currentPaymentMethod: IUpdatePaymentOptions;
        getCurrentPaymentMethod(): IUpdatePaymentOptions;
        constructor(window: Window, eventHub: IEventHub);
        getPaymentMethods(getPaymentMethodOptions: IGetPaymentMethodsOptions): Q.Promise<any>;
        updatePaymentMethod(request: IUpdatePaymentOptions): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class OnSitePOSPaymentProvider extends BaseCheckoutPaymentProvider {
        constructor(window: Window, providerName: string, eventHub: IEventHub);
        validatePayment(activeVM: IActivePaymentViewModel): Q.Promise<boolean>;
        submitPayment(activeVM: IActivePaymentViewModel): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    interface IMonerisResponseData {
        responseCode: string[];
        dataKey: string;
        errorMessage: string;
    }
}

declare module Orckestra.Composer {
    type BaseMonerisCanadaPaymentProviderCollection = {
        [type: string]: BaseSpecializedMonerisCanadaPaymentProvider;
    };
    class BaseSpecializedMonerisCanadaPaymentProvider {
        protected _window: Window;
        protected _paymentService: MonerisPaymentService;
        protected _eventHub: IEventHub;
        constructor(window: Window, paymentService: MonerisPaymentService, eventHub: IEventHub);
        registerDomEvents(): void;
        unregisterDomEvents(): void;
        validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean>;
        addVaultProfileToken(activePaymentVM: IActivePaymentViewModel): Q.Promise<any>;
    }
}

declare module Orckestra.Composer {
    class CreditCardMonerisCanadaPaymentProvider extends BaseSpecializedMonerisCanadaPaymentProvider {
        private _validationDefer;
        _monerisResponseData: IMonerisResponseData;
        _formData: any;
        constructor(window: Window, paymentService: MonerisPaymentService, eventHub: IEventHub);
        registerDomEvents(): void;
        unregisterDomEvents(): void;
        validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean>;
        private validateMonerisIFrame(vm);
        addVaultProfileToken(activePaymentVM: IActivePaymentViewModel): Q.Promise<any>;
        private handleMessageResponse(e);
        private handleMonerisSuccess(responseData);
        private handleMonerisError(monerisEvent, responseData);
        private showMonerisErrors(errorCodes);
        private collectAndValidateFormData();
        private getMonerisIFrame();
        private hideAllMonerisErrors();
        private getForm();
    }
}

declare module Orckestra.Composer {
    class SavedCreditCardMonerisCanadaPaymentProvider extends BaseSpecializedMonerisCanadaPaymentProvider {
        private _deleteModalElementSelector;
        private _busyHandler;
        private _uiModal;
        constructor(window: Window, paymentService: MonerisPaymentService, eventHub: IEventHub);
        registerDomEvents(): void;
        unregisterDomEvents(): void;
        validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean>;
        addVaultProfileToken(activePaymentVM: IActivePaymentViewModel): Q.Promise<any>;
        protected deleteCart(event: JQueryEventObject): Q.Promise<void>;
        protected releaseBusyHandler(): void;
    }
}

declare module Orckestra.Composer {
    class MonerisCanadaPaymentProvider extends BaseCheckoutPaymentProvider {
        private _validationDefer;
        private _monerisPaymentService;
        private _providers;
        constructor(window: Window, providerName: string, eventHub: IEventHub);
        providers: BaseMonerisCanadaPaymentProviderCollection;
        validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean>;
        submitPayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<any>;
        dispose(): void;
        protected setDefaultCustomerPaymentMethod(activePaymentVM: IActivePaymentViewModel): Q.Promise<IPaymentMethodViewModel>;
        protected registerSpecializedProviders(): void;
        protected registerDomEvents(): void;
        protected unregisterDomEvents(): void;
        protected getProvider(providerName: string): BaseSpecializedMonerisCanadaPaymentProvider;
    }
}

declare module Orckestra.Composer {
    class ProductSearchController extends Orckestra.Composer.Controller {
        private _debounceHandle;
        private _debounceTimeout;
        private _searchService;
        initialize(): void;
        multiFacetChanged(actionContext: IControllerActionContext): void;
        singleFacetChanged(actionContext: IControllerActionContext): void;
        removeSelectedFacet(actionContext: IControllerActionContext): void;
        clearSelectedFacets(actionContext: IControllerActionContext): void;
        private initializeSearchService();
        private buildFacetRegistry();
    }
}









declare module Orckestra.Composer {
    interface IHandlebarsLocalization extends HandlebarsStatic {
        localizationProvider: {
            handleBarsHelper_localizeFormat(categoryName: string, keyName: string, args: any): string;
            handleBarsHelper_localize(categoryName: string, keyName: string): string;
            handleBarsHelper_isLocalized(categoryName: string, keyName: string): boolean;
        };
    }
}






