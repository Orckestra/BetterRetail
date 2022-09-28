module Orckestra.Composer {
    export class ProductsHelper {
        static getKeyVariantValues(product, keyName) {
            if (!product || !product.KeyVariantAttributeItems) return [];
            const kva = product.KeyVariantAttributeItems.find(i => i.PropertyName === keyName);
            const selectedVariant = product.SelectedVariant;

            if (!kva || !selectedVariant) return [];

            const selectedKvas = selectedVariant.Kvas;
            const selectedPrValue = selectedKvas[keyName];
            const variants = product.Variants;

            const isDisabled = (relatedVariants) => {
                const otherKeyProps = product.KeyVariantAttributeItems.filter(p => p.PropertyName !== keyName);
                let disabled = false;
                otherKeyProps.forEach(otherP => {
                    const selectedOtherPrValue = selectedKvas[otherP.PropertyName];
                    if (selectedOtherPrValue) {
                        const findRelatedWithSelected = relatedVariants.find(v => v.Kvas[otherP.PropertyName] === selectedOtherPrValue);
                        if (!findRelatedWithSelected) {
                            disabled = true;
                        }
                    }
                });
                return disabled;
            }

            return kva.Values.map(prValue => {
                const relatedVariants = variants.filter(v => v.Kvas[keyName] === prValue.Value);
                return {
                    ...prValue,
                    Disabled: isDisabled(relatedVariants),
                    Selected: selectedPrValue === prValue.Value
                }
            })
        }

        static  mergeVariantPrice(product, variantPrice) {
            if(!variantPrice || !product.SizeSelected) return;

            product.IsOnSale = variantPrice.IsPriceDiscounted;
            product.DisplaySpecialPrice = variantPrice.DefaultListPrice;
            product.DisplayListPrice = variantPrice.ListPrice;
            product.HasPriceRange = false;
        }

        static findVariant(product, kva) {
            var currentVariantKvas = product.SelectedVariant.Kvas;
            const keys = Object.keys(currentVariantKvas);
            const mergedKva = { ...currentVariantKvas, ...kva };
            const compareProperties = (pr) => {
                return keys.reduce((result, current) => result && (pr[current] && pr[current] === mergedKva[current]), true);
            };
            return product.Variants.find(v => compareProperties(v.Kvas));
        }

        static  getProductDataForAnalytics(product, variantId, price, pageName, quantity: number = 1)  {
            const {
                ProductId,
                Brand,
                DisplayName,
                CategoryId
            } = product;

            let data = {
                ProductId, 
                VariantId: variantId, 
                Quantity: quantity, 
                Price: price, 
                ListPrice: price, 
                DisplayName, 
                Brand, 
                CategoryId, 
                List: pageName };

                if(variantId)
                {
                    const variant = product.Variants.find(v=> v.id === variantId);
                    const variantData = this.getVariantDataForAnalytics(variant);
                    data = {...data, ...variantData };
                }

            return data;
        }

        static getVariantDataForAnalytics(variant: any): any {
            var variantName: string = this.buildVariantName(variant.Kvas);

            var data: any = {
                Variant: variantName,
                Name: variant.DisplayName ? variant.DisplayName : undefined,
                ListPrice: variant.ListPrice
            };

            return data;
        }

        static buildVariantName(kvas: any): string {
            var keys: string[] = Object.keys(kvas).sort();
            var nameParts: string[] = [];

            for (var i: number = 0; i < keys.length; i++) {
                var key: string = keys[i];
                var value: any = kvas[key];

                nameParts.push(value);
            }

            return nameParts.join(' ');
        }
    }
}

