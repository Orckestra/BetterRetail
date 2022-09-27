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
    }
}

