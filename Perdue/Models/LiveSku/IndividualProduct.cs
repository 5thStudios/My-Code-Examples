using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



//  Generated using:
//  https://jsonformatter.org/json-to-csharp
//
//  To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//      using PerdueFS.Models;
//      var IndividualProduct = IndividualProduct.FromJson(jsonString);
//  or
//  IndividualProduct product = Newtonsoft.Json.JsonConvert.DeserializeObject<IndividualProduct>("json content goes here");



namespace www.Models.LiveSkuModel
{
    public partial class IndividualProduct
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }

        [JsonProperty("numOfRows")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long NumOfRows { get; set; }

        [JsonProperty("responseCode")]
        public string ResponseCode { get; set; }

        [JsonProperty("responseMessage")]
        public string ResponseMessage { get; set; }

        [JsonProperty("responseType")]
        public string ResponseType { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("item")]
        public Item Item { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("itemDefinitions")]
        public ItemDefinitions ItemDefinitions { get; set; }

        [JsonProperty("additionalTradeItemIdentificationGroup")]
        public AdditionalTradeItemIdentificationGroup[] AdditionalTradeItemIdentificationGroup { get; set; }

        [JsonProperty("brandOwnerGroup")]
        public BrandOwnerGroup[] BrandOwnerGroup { get; set; }

        [JsonProperty("doesTradeItemCarryUSDAChildNutritionLabel")]
        public DoesTradeItemCarryUsdaChildNutritionLabel DoesTradeItemCarryUsdaChildNutritionLabel { get; set; }

        [JsonProperty("gDSNTradeItemClassification")]
        public GDsnTradeItemClassification GDsnTradeItemClassification { get; set; }

        [JsonProperty("gtinName")]
        public GtinName GtinName { get; set; }

        [JsonProperty("informationProviderOfTradeItemGroup")]
        public InformationProviderOfTradeItemGroup[] InformationProviderOfTradeItemGroup { get; set; }

        [JsonProperty("isTradeItemABaseUnit")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemABaseUnit { get; set; }

        [JsonProperty("isTradeItemAConsumerUnit")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemAConsumerUnit { get; set; }

        [JsonProperty("isTradeItemADespatchUnitGroup")]
        public IsTradeItemADespatchUnitGroup[] IsTradeItemADespatchUnitGroup { get; set; }

        [JsonProperty("isTradeItemAService")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemAService { get; set; }

        [JsonProperty("isTradeItemAnInvoiceUnitGroup")]
        public IsTradeItemAnInvoiceUnitGroup[] IsTradeItemAnInvoiceUnitGroup { get; set; }

        [JsonProperty("isTradeItemAnOrderableUnitGroup")]
        public IsTradeItemAnOrderableUnitGroup[] IsTradeItemAnOrderableUnitGroup { get; set; }

        [JsonProperty("itemIdentificationInformation")]
        public ItemIdentificationInformation ItemIdentificationInformation { get; set; }

        [JsonProperty("manufacturerOfTradeItemGroup")]
        public ManufacturerOfTradeItemGroup[] ManufacturerOfTradeItemGroup { get; set; }

        [JsonProperty("productCategory")]
        public ProductCategory[] ProductCategory { get; set; }

        [JsonProperty("targetMarket")]
        public TargetMarket[] TargetMarket { get; set; }

        [JsonProperty("tradeItemContactInformationGroup")]
        public TradeItemContactInformationGroup[] TradeItemContactInformationGroup { get; set; }

        [JsonProperty("tradeItemInformation")]
        public TradeItemInformation[] TradeItemInformation { get; set; }

        [JsonProperty("tradeItemSynchronisationDatesGroup")]
        public TradeItemSynchronisationDatesGroup[] TradeItemSynchronisationDatesGroup { get; set; }

        [JsonProperty("tradeItemUnitDescriptorCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel TradeItemUnitDescriptorCode { get; set; }
    }

    public partial class AdditionalTradeItemIdentificationGroup
    {
        [JsonProperty("additionalTradeItemIdentification")]
        public AdditionalTradeItemIdentification[] AdditionalTradeItemIdentification { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class AdditionalTradeItemIdentification
    {
        [JsonProperty("additionalTradeItemIdentificationTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel AdditionalTradeItemIdentificationTypeCode { get; set; }

        [JsonProperty("additionalTradeItemIdentificationValue")]
        public string AdditionalTradeItemIdentificationValue { get; set; }
    }

    public partial class DoesTradeItemCarryUsdaChildNutritionLabel
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("valueDefinition")]
        public string ValueDefinition { get; set; }
    }

    public partial class Recipient
    {
        [JsonProperty("recipientId")]
        public string RecipientId { get; set; }

        [JsonProperty("recipientIdType")]
        public DoesTradeItemCarryUsdaChildNutritionLabel RecipientIdType { get; set; }
    }

    public partial class BrandOwnerGroup
    {
        [JsonProperty("brandOwner")]
        public BrandOwner BrandOwner { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class BrandOwner
    {
        [JsonProperty("informationProviderId")]
        public string InformationProviderId { get; set; }

        [JsonProperty("informationProviderType")]
        public DoesTradeItemCarryUsdaChildNutritionLabel InformationProviderType { get; set; }

        [JsonProperty("partyName")]
        public string PartyName { get; set; }

        [JsonProperty("partyAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string PartyAddress { get; set; }
    }

    public partial class GDsnTradeItemClassification
    {
        [JsonProperty("gpcCategoryCode")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long GpcCategoryCode { get; set; }

        [JsonProperty("gpcCategoryDefinition")]
        public string GpcCategoryDefinition { get; set; }
    }

    public partial class GtinName
    {
        [JsonProperty("values")]
        public GtinNameValue[] Values { get; set; }

        [JsonProperty("languageDefinition")]
        public LanguageDefinition LanguageDefinition { get; set; }
    }

    public partial class GtinNameValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }
    }

    public partial class InformationProviderOfTradeItemGroup
    {
        [JsonProperty("informationProviderOfTradeItem")]
        public BrandOwner InformationProviderOfTradeItem { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class IsTradeItemADespatchUnitGroup
    {
        [JsonProperty("isTradeItemADespatchUnit")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemADespatchUnit { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class IsTradeItemAnInvoiceUnitGroup
    {
        [JsonProperty("isTradeItemAnInvoiceUnit")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemAnInvoiceUnit { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class IsTradeItemAnOrderableUnitGroup
    {
        [JsonProperty("isTradeItemAnOrderableUnit")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemAnOrderableUnit { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class ItemDefinitions
    {
        [JsonProperty("definitions")]
        public Definition[] Definitions { get; set; }
    }

    public partial class Definition
    {
        [JsonProperty("values")]
        public DefinitionValue[] Values { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public TypeEnum Type { get; set; }
    }

    public partial class DefinitionValue
    {
        [JsonProperty("languageValues")]
        public LanguageValue[] LanguageValues { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public partial class LanguageValue
    {
        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class ItemIdentificationInformation
    {
        [JsonProperty("informationProvider")]
        public InformationProvider[] InformationProvider { get; set; }

        [JsonProperty("itemIdentifier")]
        public ItemIdentifier[] ItemIdentifier { get; set; }

        [JsonProperty("itemReferenceIdInformation")]
        public ItemReferenceIdInformation ItemReferenceIdInformation { get; set; }

        [JsonProperty("targetMarket")]
        public DoesTradeItemCarryUsdaChildNutritionLabel TargetMarket { get; set; }
    }

    public partial class InformationProvider
    {
        [JsonProperty("informationProviderId")]
        public string InformationProviderId { get; set; }

        [JsonProperty("informationProviderIdType")]
        public DoesTradeItemCarryUsdaChildNutritionLabel InformationProviderIdType { get; set; }

        [JsonProperty("informationProviderName")]
        public string InformationProviderName { get; set; }

        [JsonProperty("isPrimary")]
        public string IsPrimary { get; set; }
    }

    public partial class ItemIdentifier
    {
        [JsonProperty("isPrimary")]
        public string IsPrimary { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [JsonProperty("itemIdType")]
        public DoesTradeItemCarryUsdaChildNutritionLabel ItemIdType { get; set; }
    }

    public partial class ItemReferenceIdInformation
    {
        [JsonProperty("itemReferenceId")]
        public string ItemReferenceId { get; set; }
    }

    public partial class ManufacturerOfTradeItemGroup
    {
        [JsonProperty("manufacturerOfTradeItem")]
        public BrandOwner[] ManufacturerOfTradeItem { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class ProductCategory
    {
        [JsonProperty("productCategoryCodes")]
        public ProductCategoryCode[] ProductCategoryCodes { get; set; }

        [JsonProperty("productCategoryScheme")]
        public DoesTradeItemCarryUsdaChildNutritionLabel ProductCategoryScheme { get; set; }
    }

    public partial class ProductCategoryCode
    {
        [JsonProperty("productCategoryCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel ProductCategoryCodeProductCategoryCode { get; set; }

        [JsonProperty("productCategoryComponent")]
        public DoesTradeItemCarryUsdaChildNutritionLabel ProductCategoryComponent { get; set; }
    }

    public partial class TargetMarket
    {
        [JsonProperty("targetMarketCountryCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel TargetMarketCountryCode { get; set; }
    }

    public partial class TradeItemContactInformationGroup
    {
        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }

        [JsonProperty("tradeItemContactInformation")]
        public TradeItemContactInformation[] TradeItemContactInformation { get; set; }
    }

    public partial class TradeItemContactInformation
    {
        [JsonProperty("contactTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel ContactTypeCode { get; set; }

        [JsonProperty("informationProviderType")]
        public DoesTradeItemCarryUsdaChildNutritionLabel InformationProviderType { get; set; }

        [JsonProperty("targetMarketCommunicationChannel")]
        public TargetMarketCommunicationChannel[] TargetMarketCommunicationChannel { get; set; }
    }

    public partial class TargetMarketCommunicationChannel
    {
        [JsonProperty("communicationChannel")]
        public CommunicationChannel[] CommunicationChannel { get; set; }
    }

    public partial class CommunicationChannel
    {
        [JsonProperty("communicationChannelCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel CommunicationChannelCode { get; set; }

        [JsonProperty("communicationValue")]
        public string CommunicationValue { get; set; }
    }

    public partial class TradeItemInformation
    {
        [JsonProperty("allergenInformationModuleGroup")]
        public AllergenInformationModuleGroup[] AllergenInformationModuleGroup { get; set; }

        [JsonProperty("consumerInstructionsModule")]
        public ConsumerInstructionsModule[] ConsumerInstructionsModule { get; set; }

        [JsonProperty("deliveryPurchasingInformationModuleGroup")]
        public DeliveryPurchasingInformationModuleGroup[] DeliveryPurchasingInformationModuleGroup { get; set; }

        [JsonProperty("dietInformationModuleGroup")]
        public DietInformationModuleGroup[] DietInformationModuleGroup { get; set; }

        [JsonProperty("farmingAndProcessingInformationModule")]
        public FarmingAndProcessingInformationModule FarmingAndProcessingInformationModule { get; set; }

        [JsonProperty("foodAndBeverageIngredientModule")]
        public FoodAndBeverageIngredientModule[] FoodAndBeverageIngredientModule { get; set; }

        [JsonProperty("foodAndBeveragePreparationServingModule")]
        public FoodAndBeveragePreparationServingModule[] FoodAndBeveragePreparationServingModule { get; set; }

        [JsonProperty("marketingInformationModuleGroup")]
        public MarketingInformationModuleGroup[] MarketingInformationModuleGroup { get; set; }

        [JsonProperty("nutritionalInformationModule")]
        public NutritionalInformationModule[] NutritionalInformationModule { get; set; }

        [JsonProperty("packagingMarkingModule")]
        public PackagingMarkingModule PackagingMarkingModule { get; set; }

        [JsonProperty("placeOfItemActivityModule")]
        public PlaceOfItemActivityModule PlaceOfItemActivityModule { get; set; }

        [JsonProperty("productNameGroup")]
        public ProductNameGroup[] ProductNameGroup { get; set; }

        [JsonProperty("referencedFileDetailInformationModule")]
        public ReferencedFileDetailInformationModule ReferencedFileDetailInformationModule { get; set; }

        [JsonProperty("tradeItemDataCarrierAndIdentificationModule")]
        public TradeItemDataCarrierAndIdentificationModule TradeItemDataCarrierAndIdentificationModule { get; set; }

        [JsonProperty("tradeItemDescriptionModule")]
        public TradeItemDescriptionModule TradeItemDescriptionModule { get; set; }

        [JsonProperty("tradeItemHierarchyModuleGroup")]
        public TradeItemHierarchyModuleGroup[] TradeItemHierarchyModuleGroup { get; set; }

        [JsonProperty("tradeItemLifespanModuleGroup")]
        public TradeItemLifespanModuleGroup[] TradeItemLifespanModuleGroup { get; set; }

        [JsonProperty("tradeItemMeasurementsModuleGroup")]
        public TradeItemMeasurementsModuleGroup[] TradeItemMeasurementsModuleGroup { get; set; }

        //[JsonProperty("tradeItemSizeModule")]
        //public TradeItemSizeModule[] TradeItemSizeModule { get; set; }

        [JsonProperty("tradeItemTemperatureInformationModule")]
        public TradeItemTemperatureInformationModule TradeItemTemperatureInformationModule { get; set; }

        [JsonProperty("variableTradeItemInformationModule")]
        public VariableTradeItemInformationModule VariableTradeItemInformationModule { get; set; }



        ////JF | 2022-04-19
        //[JsonProperty("tradeItemSizeModule")]
        //public TradeItemSizeModule TradeItemSizeModule { get; set; }
    }

    public partial class AllergenInformationModuleGroup
    {
        [JsonProperty("allergenInformationModule")]
        public AllergenInformationModule AllergenInformationModule { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class AllergenInformationModule
    {
        [JsonProperty("allergenRelatedInformation")]
        public AllergenRelatedInformation[] AllergenRelatedInformation { get; set; }
    }

    public partial class AllergenRelatedInformation
    {
        [JsonProperty("allergen")]
        public Allergen[] Allergen { get; set; }

        [JsonProperty("allergenSpecificationAgency")]
        public string AllergenSpecificationAgency { get; set; }

        [JsonProperty("allergenSpecificationName")]
        public string AllergenSpecificationName { get; set; }

        [JsonProperty("isAllergenRelevantDataProvided")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsAllergenRelevantDataProvided { get; set; }
    }

    public partial class Allergen
    {
        [JsonProperty("allergenTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel AllergenTypeCode { get; set; }

        [JsonProperty("levelOfContainmentCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel LevelOfContainmentCode { get; set; }
    }

    public partial class ConsumerInstructionsModule
    {
        [JsonProperty("consumerInstructions")]
        public ConsumerInstructions ConsumerInstructions { get; set; }
    }

    public partial class ConsumerInstructions
    {
        [JsonProperty("consumerStorageInstructions")]
        public GtinName ConsumerStorageInstructions { get; set; }
    }

    public partial class DeliveryPurchasingInformationModuleGroup
    {
        [JsonProperty("deliveryPurchasingInformationModule")]
        public DeliveryPurchasingInformationModule[] DeliveryPurchasingInformationModule { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class DeliveryPurchasingInformationModule
    {
        [JsonProperty("deliveryPurchasingInformation")]
        public DeliveryPurchasingInformation DeliveryPurchasingInformation { get; set; }
    }

    public partial class DeliveryPurchasingInformation
    {
        [JsonProperty("firstOrderDateTime")]
        public DateTimeOffset FirstOrderDateTime { get; set; }

        [JsonProperty("firstShipDateTime")]
        public DateTimeOffset FirstShipDateTime { get; set; }

        [JsonProperty("startAvailabilityDateTime")]
        public DateTimeOffset StartAvailabilityDateTime { get; set; }
    }

    public partial class DietInformationModuleGroup
    {
        [JsonProperty("dietInformationModule")]
        public DietInformationModule DietInformationModule { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class DietInformationModule
    {
        [JsonProperty("dietInformation")]
        public DietInformation[] DietInformation { get; set; }
    }

    public partial class DietInformation
    {
        [JsonProperty("dietTypeInformation")]
        public DietTypeInformation[] DietTypeInformation { get; set; }
    }

    public partial class DietTypeInformation
    {
        [JsonProperty("dietCertification")]
        public DietCertification[] DietCertification { get; set; }

        [JsonProperty("dietTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel DietTypeCode { get; set; }
    }

    public partial class DietCertification
    {
        [JsonProperty("additionalCertificationOrganisationIdentifier")]
        public AdditionalCertificationOrganisationIdentifier[] AdditionalCertificationOrganisationIdentifier { get; set; }

        [JsonProperty("certification")]
        public Certification[] Certification { get; set; }
    }

    public partial class AdditionalCertificationOrganisationIdentifier
    {
        [JsonProperty("informationProviderIdType")]
        public DoesTradeItemCarryUsdaChildNutritionLabel InformationProviderIdType { get; set; }
    }

    public partial class Certification
    {
        [JsonProperty("certificationValue")]
        public string CertificationValue { get; set; }
    }

    public partial class FarmingAndProcessingInformationModule
    {
        [JsonProperty("tradeItemFarmingAndProcessing")]
        public TradeItemFarmingAndProcessing TradeItemFarmingAndProcessing { get; set; }

        [JsonProperty("tradeItemOrganicInformation")]
        public TradeItemOrganicInformation TradeItemOrganicInformation { get; set; }
    }

    public partial class TradeItemFarmingAndProcessing
    {
        [JsonProperty("geneticallyModifiedDeclarationCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel GeneticallyModifiedDeclarationCode { get; set; }

        [JsonProperty("growingMethodCode")]
        public GrowingMethodCode GrowingMethodCode { get; set; }

        [JsonProperty("irradiatedCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel IrradiatedCode { get; set; }
    }

    public partial class GrowingMethodCode
    {
        [JsonProperty("values")]
        public string[] Values { get; set; }

        [JsonProperty("valueDefinition")]
        public string ValueDefinition { get; set; }
    }

    public partial class TradeItemOrganicInformation
    {
        [JsonProperty("organicClaim")]
        public OrganicClaim[] OrganicClaim { get; set; }
    }

    public partial class OrganicClaim
    {
        [JsonProperty("organicTradeItemCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel OrganicTradeItemCode { get; set; }
    }

    public partial class FoodAndBeverageIngredientModule
    {
        [JsonProperty("foodAndBeverageIngredient")]
        public FoodAndBeverageIngredient[] FoodAndBeverageIngredient { get; set; }

        [JsonProperty("ingredientStatement")]
        public IngredientStatement[] IngredientStatement { get; set; }
    }

    public partial class FoodAndBeverageIngredient
    {
        [JsonProperty("ingredientName")]
        public GtinName IngredientName { get; set; }

        [JsonProperty("ingredientSequence")]
        public string IngredientSequence { get; set; }
    }

    public partial class IngredientStatement
    {
        [JsonProperty("statement")]
        public GtinName Statement { get; set; }
    }

    public partial class FoodAndBeveragePreparationServingModule
    {
        [JsonProperty("preparationServing")]
        public PreparationServing[] PreparationServing { get; set; }

        [JsonProperty("servingQuantityInformation")]
        public ServingQuantityInformation[] ServingQuantityInformation { get; set; }
    }

    public partial class PreparationServing
    {
        [JsonProperty("preparationInstructions")]
        public GtinName PreparationInstructions { get; set; }

        [JsonProperty("preparationTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel PreparationTypeCode { get; set; }

        [JsonProperty("servingSuggestion")]
        public GtinName ServingSuggestion { get; set; }
    }

    public partial class ServingQuantityInformation
    {
        [JsonProperty("numberOfServingsPerPackage")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long NumberOfServingsPerPackage { get; set; }
    }

    public partial class MarketingInformationModuleGroup
    {
        [JsonProperty("marketingInformationModule")]
        public MarketingInformationModule MarketingInformationModule { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class MarketingInformationModule
    {
        [JsonProperty("marketingInformation")]
        public MarketingInformation MarketingInformation { get; set; }
    }

    public partial class MarketingInformation
    {
        [JsonProperty("tradeItemKeyWords")]
        public GtinName TradeItemKeyWords { get; set; }

        [JsonProperty("tradeItemMarketingMessage")]
        public TradeItemMarketingMessage[] TradeItemMarketingMessage { get; set; }
    }

    public partial class TradeItemMarketingMessage
    {
        [JsonProperty("tradeItemMarketingMessage")]
        public GtinName TradeItemMarketingMessageTradeItemMarketingMessage { get; set; }
    }

    public partial class NutritionalInformationModule
    {
        [JsonProperty("nutrientFormatTypeCodeReference")]
        public NutrientFormatTypeCodeReference[] NutrientFormatTypeCodeReference { get; set; }

        [JsonProperty("nutrientHeader")]
        public NutrientHeader[] NutrientHeader { get; set; }

        [JsonProperty("nutritionalClaimDetail")]
        public NutritionalClaimDetail[] NutritionalClaimDetail { get; set; }
    }

    public partial class NutrientFormatTypeCodeReference
    {
        [JsonProperty("nutrientFormatTypeCodeReferenceValue")]
        public string NutrientFormatTypeCodeReferenceValue { get; set; }
    }

    public partial class NutrientHeader
    {
        [JsonProperty("nutrientBasisQuantity")]
        public MaximumTemperature NutrientBasisQuantity { get; set; }

        [JsonProperty("nutrientBasisQuantityTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel NutrientBasisQuantityTypeCode { get; set; }

        [JsonProperty("nutrientDetail")]
        public NutrientDetail[] NutrientDetail { get; set; }

        [JsonProperty("preparationStateCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel PreparationStateCode { get; set; }

        [JsonProperty("servingSize")]
        public ServingSize ServingSize { get; set; }

        [JsonProperty("servingSizeDescription")]
        public GtinName ServingSizeDescription { get; set; }
    }

    public partial class MaximumTemperature
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("qual")]
        public string Qual { get; set; }

        [JsonProperty("qualDefinition")]
        public QualDefinition QualDefinition { get; set; }
    }

    public partial class NutrientDetail
    {
        [JsonProperty("measurementPrecisionCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel MeasurementPrecisionCode { get; set; }

        [JsonProperty("nutrientTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel NutrientTypeCode { get; set; }

        [JsonProperty("quantityContained", NullValueHandling = NullValueHandling.Ignore)]
        public ServingSize QuantityContained { get; set; }

        [JsonProperty("dailyValueIntakePercent", NullValueHandling = NullValueHandling.Ignore)]
        public string DailyValueIntakePercent { get; set; }
    }

    public partial class ServingSize
    {
        [JsonProperty("values")]
        public ServingSizeValue[] Values { get; set; }

        [JsonProperty("qualDefinition")]
        public QualDefinition QualDefinition { get; set; }
    }

    public partial class ServingSizeValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("qual")]
        public Qual Qual { get; set; }
    }

    public partial class NutritionalClaimDetail
    {
        [JsonProperty("nutritionalClaimNutrientElementCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel NutritionalClaimNutrientElementCode { get; set; }

        [JsonProperty("nutritionalClaimTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel NutritionalClaimTypeCode { get; set; }
    }

    public partial class PackagingMarkingModule
    {
        [JsonProperty("packagingMarking")]
        public PackagingMarking[] PackagingMarking { get; set; }
    }

    public partial class PackagingMarking
    {
        [JsonProperty("hasBatchNumber")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool HasBatchNumber { get; set; }

        [JsonProperty("isPackagingMarkedReturnable")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsPackagingMarkedReturnable { get; set; }

        [JsonProperty("isTradeItemMarkedAsRecyclable")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemMarkedAsRecyclable { get; set; }

        [JsonProperty("packagingDate")]
        public PackagingDate[] PackagingDate { get; set; }
    }

    public partial class PackagingDate
    {
        [JsonProperty("tradeItemDateOnPackagingTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel TradeItemDateOnPackagingTypeCode { get; set; }
    }

    public partial class PlaceOfItemActivityModule
    {
        [JsonProperty("placeOfProductActivity")]
        public PlaceOfProductActivity PlaceOfProductActivity { get; set; }
    }

    public partial class PlaceOfProductActivity
    {
        [JsonProperty("countryOfOrigin")]
        public CountryOfOrigin[] CountryOfOrigin { get; set; }
    }

    public partial class CountryOfOrigin
    {
        [JsonProperty("countryCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel CountryCode { get; set; }
    }

    public partial class ProductNameGroup
    {
        [JsonProperty("productName")]
        public GtinName ProductName { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }

    public partial class ReferencedFileDetailInformationModule
    {
        [JsonProperty("referencedFileHeader")]
        public ReferencedFileHeader[] ReferencedFileHeader { get; set; }
    }

    public partial class ReferencedFileHeader
    {
        [JsonProperty("fileEffectiveStartDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? FileEffectiveStartDateTime { get; set; }

        [JsonProperty("fileFormatDescription")]
        public GtinName FileFormatDescription { get; set; }

        [JsonProperty("fileFormatName")]
        public string FileFormatName { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileVersion")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long FileVersion { get; set; }

        [JsonProperty("imageSource")]
        public DoesTradeItemCarryUsdaChildNutritionLabel ImageSource { get; set; }

        [JsonProperty("isPrimaryFile")]
        public DoesTradeItemCarryUsdaChildNutritionLabel IsPrimaryFile { get; set; }

        [JsonProperty("referencedFileDetail")]
        public ReferencedFileDetail ReferencedFileDetail { get; set; }

        [JsonProperty("referencedFileTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel ReferencedFileTypeCode { get; set; }

        [JsonProperty("shareType")]
        public string ShareType { get; set; }

        [JsonProperty("uniformResourceIdentifier")]
        public Uri UniformResourceIdentifier { get; set; }
    }

    public partial class ReferencedFileDetail
    {
        [JsonProperty("filePixelHeight")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long FilePixelHeight { get; set; }

        [JsonProperty("filePixelWidth")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long FilePixelWidth { get; set; }

        [JsonProperty("fileSize")]
        public ServingSize FileSize { get; set; }

        [JsonProperty("fileUsageInformation")]
        public FileUsageInformation FileUsageInformation { get; set; }
    }

    public partial class FileUsageInformation
    {
        [JsonProperty("isFileForInternalUseOnly")]
        public DoesTradeItemCarryUsdaChildNutritionLabel IsFileForInternalUseOnly { get; set; }

        [JsonProperty("isTalentReleaseOnFile")]
        public DoesTradeItemCarryUsdaChildNutritionLabel IsTalentReleaseOnFile { get; set; }
    }

    public partial class TradeItemDataCarrierAndIdentificationModule
    {
        [JsonProperty("dataCarrier")]
        public DataCarrier[] DataCarrier { get; set; }

        [JsonProperty("gs1TradeItemIdentificationKey")]
        public Gs1TradeItemIdentificationKey[] Gs1TradeItemIdentificationKey { get; set; }
    }

    public partial class DataCarrier
    {
        [JsonProperty("dataCarrierTypeCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel DataCarrierTypeCode { get; set; }
    }

    public partial class Gs1TradeItemIdentificationKey
    {
        [JsonProperty("gs1TradeItemIdentificationKeyCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel Gs1TradeItemIdentificationKeyCode { get; set; }

        [JsonProperty("gs1TradeItemIdentificationKeyValue")]
        public string Gs1TradeItemIdentificationKeyValue { get; set; }
    }

    public partial class TradeItemDescriptionModule
    {
        [JsonProperty("tradeItemDescriptionInformation")]
        public TradeItemDescriptionInformation[] TradeItemDescriptionInformation { get; set; }
    }

    public partial class TradeItemDescriptionInformation
    {
        [JsonProperty("additionalTradeItemDescription")]
        public GtinName AdditionalTradeItemDescription { get; set; }

        [JsonProperty("brandNameInformation")]
        public BrandNameInformation BrandNameInformation { get; set; }

        [JsonProperty("descriptionShort")]
        public GtinName DescriptionShort { get; set; }

        [JsonProperty("functionalName")]
        public GtinName FunctionalName { get; set; }
    }

    public partial class BrandNameInformation
    {
        [JsonProperty("brandName")]
        public string BrandName { get; set; }
    }

    public partial class TradeItemHierarchyModuleGroup
    {
        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }

        [JsonProperty("tradeItemHierarchyModule")]
        public TradeItemHierarchyModule TradeItemHierarchyModule { get; set; }
    }

    public partial class TradeItemHierarchyModule
    {
        [JsonProperty("tradeItemHierarchy")]
        public TradeItemHierarchy TradeItemHierarchy { get; set; }
    }

    public partial class TradeItemHierarchy
    {
        [JsonProperty("quantityOfCompleteLayersContainedInATradeItem")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long QuantityOfCompleteLayersContainedInATradeItem { get; set; }

        [JsonProperty("quantityOfInnerPack")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long QuantityOfInnerPack { get; set; }

        [JsonProperty("quantityOfLayersPerPallet")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long QuantityOfLayersPerPallet { get; set; }

        [JsonProperty("quantityOfTradeItemsContainedInACompleteLayer")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long QuantityOfTradeItemsContainedInACompleteLayer { get; set; }

        [JsonProperty("quantityOfTradeItemsPerPallet")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long QuantityOfTradeItemsPerPallet { get; set; }

        [JsonProperty("quantityOfTradeItemsPerPalletLayer")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long QuantityOfTradeItemsPerPalletLayer { get; set; }
    }

    public partial class TradeItemLifespanModuleGroup
    {
        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }

        [JsonProperty("tradeItemLifespanModule")]
        public TradeItemLifespanModule[] TradeItemLifespanModule { get; set; }
    }

    public partial class TradeItemLifespanModule
    {
        [JsonProperty("tradeItemLifespan")]
        public TradeItemLifespan TradeItemLifespan { get; set; }
    }

    public partial class TradeItemLifespan
    {
        [JsonProperty("minimumTradeItemLifespanFromTimeOfArrival")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long MinimumTradeItemLifespanFromTimeOfArrival { get; set; }

        [JsonProperty("minimumTradeItemLifespanFromTimeOfProduction")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public long MinimumTradeItemLifespanFromTimeOfProduction { get; set; }
    }

    public partial class TradeItemMeasurementsModuleGroup
    {
        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }

        [JsonProperty("tradeItemMeasurementsModule")]
        public TradeItemMeasurementsModule TradeItemMeasurementsModule { get; set; }
    }

    public partial class TradeItemMeasurementsModule
    {
        [JsonProperty("tradeItemMeasurements")]
        public TradeItemMeasurements TradeItemMeasurements { get; set; }
    }

    public partial class TradeItemMeasurements
    {
        [JsonProperty("depth")]
        public MaximumTemperature Depth { get; set; }

        [JsonProperty("height")]
        public MaximumTemperature Height { get; set; }

        [JsonProperty("inBoxCubeDimension")]
        public MaximumTemperature InBoxCubeDimension { get; set; }

        [JsonProperty("individualUnitMaximumSize")]
        public MaximumTemperature IndividualUnitMaximumSize { get; set; }

        [JsonProperty("individualUnitMinimumSize")]
        public MaximumTemperature IndividualUnitMinimumSize { get; set; }

        [JsonProperty("netContent")]
        public ServingSize NetContent { get; set; }

        [JsonProperty("tradeItemWeight")]
        public TradeItemWeight TradeItemWeight { get; set; }

        [JsonProperty("width")]
        public MaximumTemperature Width { get; set; }
    }

    public partial class TradeItemWeight
    {
        [JsonProperty("grossWeight")]
        public MaximumTemperature GrossWeight { get; set; }

        [JsonProperty("netWeight")]
        public MaximumTemperature NetWeight { get; set; }
    }

    public partial class TradeItemTemperatureInformationModule
    {
        [JsonProperty("tradeItemTemperatureInformation")]
        public TradeItemTemperatureInformation[] TradeItemTemperatureInformation { get; set; }
    }

    public partial class TradeItemTemperatureInformation
    {
        [JsonProperty("maximumTemperature")]
        public MaximumTemperature MaximumTemperature { get; set; }

        [JsonProperty("minimumTemperature")]
        public MaximumTemperature MinimumTemperature { get; set; }

        [JsonProperty("temperatureQualifierCode")]
        public DoesTradeItemCarryUsdaChildNutritionLabel TemperatureQualifierCode { get; set; }
    }

    public partial class VariableTradeItemInformationModule
    {
        [JsonProperty("variableTradeItemInformation")]
        public VariableTradeItemInformation VariableTradeItemInformation { get; set; }
    }

    public partial class VariableTradeItemInformation
    {
        [JsonProperty("isTradeItemAVariableUnit")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool IsTradeItemAVariableUnit { get; set; }
    }

    public partial class TradeItemSynchronisationDatesGroup
    {
        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }

        [JsonProperty("tradeItemSynchronisationDates")]
        public TradeItemSynchronisationDates TradeItemSynchronisationDates { get; set; }
    }

    public partial class TradeItemSynchronisationDates
    {
        [JsonProperty("effectiveDateTime")]
        public DateTimeOffset EffectiveDateTime { get; set; }

        [JsonProperty("lastChangeDateTime")]
        public DateTimeOffset LastChangeDateTime { get; set; }

        [JsonProperty("publicationDateTime")]
        public DateTimeOffset PublicationDateTime { get; set; }
    }

    public enum LanguageDefinition { LanguageDefinition };

    public enum Language { En };

    public enum TypeEnum { Qual, Value };

    public enum QualDefinition { UomDefinition };

    public enum Qual { Ad, E14, Grm, Lbr, Mc, Mgm, NIU };

    public partial class IndividualProduct
    {
        public static IndividualProduct FromJson(string json) => JsonConvert.DeserializeObject<IndividualProduct>(json, www.Models.LiveSkuModel.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this IndividualProduct self) => JsonConvert.SerializeObject(self, www.Models.LiveSkuModel.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                LanguageDefinitionConverter.Singleton,
                LanguageConverter.Singleton,
                TypeEnumConverter.Singleton,
                QualDefinitionConverter.Singleton,
                QualConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class PurpleParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly PurpleParseStringConverter Singleton = new PurpleParseStringConverter();
    }

    internal class LanguageDefinitionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(LanguageDefinition) || t == typeof(LanguageDefinition?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "languageDefinition")
            {
                return LanguageDefinition.LanguageDefinition;
            }
            throw new Exception("Cannot unmarshal type LanguageDefinition");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (LanguageDefinition)untypedValue;
            if (value == LanguageDefinition.LanguageDefinition)
            {
                serializer.Serialize(writer, "languageDefinition");
                return;
            }
            throw new Exception("Cannot marshal type LanguageDefinition");
        }

        public static readonly LanguageDefinitionConverter Singleton = new LanguageDefinitionConverter();
    }

    internal class LanguageConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Language) || t == typeof(Language?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "en")
            {
                return Language.En;
            }
            throw new Exception("Cannot unmarshal type Language");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Language)untypedValue;
            if (value == Language.En)
            {
                serializer.Serialize(writer, "en");
                return;
            }
            throw new Exception("Cannot marshal type Language");
        }

        public static readonly LanguageConverter Singleton = new LanguageConverter();
    }

    internal class FluffyParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            bool b;
            if (Boolean.TryParse(value, out b))
            {
                return b;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (bool)untypedValue;
            var boolString = value ? "true" : "false";
            serializer.Serialize(writer, boolString);
            return;
        }

        public static readonly FluffyParseStringConverter Singleton = new FluffyParseStringConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "QUAL":
                    return TypeEnum.Qual;
                case "VALUE":
                    return TypeEnum.Value;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Qual:
                    serializer.Serialize(writer, "QUAL");
                    return;
                case TypeEnum.Value:
                    serializer.Serialize(writer, "VALUE");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }

    internal class QualDefinitionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(QualDefinition) || t == typeof(QualDefinition?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "uomDefinition")
            {
                return QualDefinition.UomDefinition;
            }
            throw new Exception("Cannot unmarshal type QualDefinition");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (QualDefinition)untypedValue;
            if (value == QualDefinition.UomDefinition)
            {
                serializer.Serialize(writer, "uomDefinition");
                return;
            }
            throw new Exception("Cannot marshal type QualDefinition");
        }

        public static readonly QualDefinitionConverter Singleton = new QualDefinitionConverter();
    }

    internal class QualConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Qual) || t == typeof(Qual?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "AD":
                    return Qual.Ad;
                case "E14":
                    return Qual.E14;
                case "GRM":
                    return Qual.Grm;
                case "LBR":
                    return Qual.Lbr;
                case "MC":
                    return Qual.Mc;
                case "MGM":
                    return Qual.Mgm;
            }
            throw new Exception("Cannot unmarshal type Qual");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Qual)untypedValue;
            switch (value)
            {
                case Qual.Ad:
                    serializer.Serialize(writer, "AD");
                    return;
                case Qual.E14:
                    serializer.Serialize(writer, "E14");
                    return;
                case Qual.Grm:
                    serializer.Serialize(writer, "GRM");
                    return;
                case Qual.Lbr:
                    serializer.Serialize(writer, "LBR");
                    return;
                case Qual.Mc:
                    serializer.Serialize(writer, "MC");
                    return;
                case Qual.Mgm:
                    serializer.Serialize(writer, "MGM");
                    return;
            }
            throw new Exception("Cannot marshal type Qual");
        }

        public static readonly QualConverter Singleton = new QualConverter();
    }
}
