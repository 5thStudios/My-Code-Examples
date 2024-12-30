using Newtonsoft.Json;
using System.Text.Json.Serialization;

// www.ReadApi.Result myDeserializedClass = JsonConvert.DeserializeObject<www.ReadApi.Result>(myJsonResponse);

namespace www.ReadApi
{
    public class Result
    {
        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("items")]
        public List<Item>? Items { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonProperty("searchAfter", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("searchAfter")]
        public List<object>? SearchAfter { get; set; }
    }

    public class AdditionalDescription
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Allergen
    {
        [JsonProperty("allergenTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("allergenTypeCode")]
        public string? AllergenTypeCode { get; set; }

        [JsonProperty("levelOfContainmentCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("levelOfContainmentCode")]
        public string? LevelOfContainmentCode { get; set; }
    }

    public class AllergenRelatedInformation
    {
        [JsonProperty("allergenSpecificationName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("allergenSpecificationName")]
        public string? AllergenSpecificationName { get; set; }

        [JsonProperty("allergen", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("allergen")]
        public List<Allergen>? Allergen { get; set; }

        [JsonProperty("isAllergenRelevantDataProvided", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isAllergenRelevantDataProvided")]
        public string? IsAllergenRelevantDataProvided { get; set; }

        [JsonProperty("allergenSpecificationAgency", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("allergenSpecificationAgency")]
        public string? AllergenSpecificationAgency { get; set; }
    }

    public class AlternateItemIdentification
    {
        [JsonProperty("agency", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("agency")]
        public string? Agency { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    public class BaseUnitsPerPallet
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Certification
    {
        [JsonProperty("certificationValue", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("certificationValue")]
        public string? CertificationValue { get; set; }
    }

    public class ClaimDetail
    {
        [JsonProperty("claimTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("claimTypeCode")]
        public string? ClaimTypeCode { get; set; }

        [JsonProperty("claimElementCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("claimElementCode")]
        public string? ClaimElementCode { get; set; }
    }

    public class CommunicationChannel
    {
        [JsonProperty("communicationChannelNumber", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("communicationChannelNumber")]
        public string? CommunicationChannelNumber { get; set; }

        [JsonProperty("communicationChannelCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("communicationChannelCode")]
        public string? CommunicationChannelCode { get; set; }
    }

    public class ConsumerStorageInstruction
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class CountryOfOrigin
    {
        [JsonProperty("countryCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }
    }

    public class CountryOfOriginStatement
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Dam
    {
        [JsonProperty("general", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("general")]
        public General? General { get; set; }

        [JsonProperty("a1c1FileName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("a1c1FileName")]
        public string? A1c1FileName { get; set; }

        [JsonProperty("primaryImage", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("primaryImage")]
        public string? PrimaryImage { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("description")]
        public Description? Description { get; set; }

        [JsonProperty("imageInfo", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imageInfo")]
        public ImageInfo? ImageInfo { get; set; }

        [JsonProperty("excludedRecipients", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("excludedRecipients")]
        public List<object>? ExcludedRecipients { get; set; }

        [JsonProperty("shareType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("shareType")]
        public string? ShareType { get; set; }
    }

    public class DataCarrier
    {
        [JsonProperty("dataCarrierTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("dataCarrierTypeCode")]
        public string? DataCarrierTypeCode { get; set; }
    }

    public class Depth
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Description
    {
        [JsonProperty("fileFormatDescription", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileFormatDescription")]
        public List<FileFormatDescription>? FileFormatDescription { get; set; }

        [JsonProperty("fileResolutionDescription", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileResolutionDescription")]
        public List<FileResolutionDescription>? FileResolutionDescription { get; set; }
    }

    public class DescriptiveSizeDimension
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class DistributionDetail
    {
        [JsonProperty("isDistributionMethodPrimary", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isDistributionMethodPrimary")]
        public string? IsDistributionMethodPrimary { get; set; }

        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("orderingLeadTime", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("orderingLeadTime")]
        public List<OrderingLeadTime>? OrderingLeadTime { get; set; }

        [JsonProperty("deliveryFrequencyCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("deliveryFrequencyCode")]
        public string? DeliveryFrequencyCode { get; set; }

        [JsonProperty("distributionMethodCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("distributionMethodCode")]
        public string? DistributionMethodCode { get; set; }
    }

    public class ExternalFileLink
    {
        [JsonProperty("imageFacing", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imageFacing")]
        public string? ImageFacing { get; set; }

        [JsonProperty("imageSource", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imageSource")]
        public string? ImageSource { get; set; }

        [JsonProperty("primaryImage", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("primaryImage")]
        public string? PrimaryImage { get; set; }

        [JsonProperty("fileEffectiveStartDateTime", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileEffectiveStartDateTime")]
        public DateTime? FileEffectiveStartDateTime { get; set; }

        [JsonProperty("imageBackground", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imageBackground")]
        public string? ImageBackground { get; set; }

        [JsonProperty("typeOfInformation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("typeOfInformation")]
        public string? TypeOfInformation { get; set; }

        [JsonProperty("imageOrientationTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imageOrientationTypeCode")]
        public string? ImageOrientationTypeCode { get; set; }

        [JsonProperty("sharedWith", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("sharedWith")]
        public List<string>? SharedWith { get; set; }
    }

    public class FileFormatDescription
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class FileResolutionDescription
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class FileSize
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class FirstOrderDate
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public DateTime? Value { get; set; }
    }

    public class FirstShipDate
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public DateTime? Value { get; set; }
    }

    public class FoodAndBevDietTypeInfo
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("dietTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("dietTypeCode")]
        public string? DietTypeCode { get; set; }

        [JsonProperty("certification", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("certification")]
        public List<Certification>? Certification { get; set; }
    }

    public class FoodAndBevIngredient
    {
        [JsonProperty("ingredientName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ingredientName")]
        public List<IngredientName>? IngredientName { get; set; }

        [JsonProperty("ingredientSequence", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ingredientSequence")]
        public string? IngredientSequence { get; set; }
    }

    public class FoodAndBevPreparationInfo
    {
        [JsonProperty("servingSuggestion", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("servingSuggestion")]
        public List<ServingSuggestion>? ServingSuggestion { get; set; }

        [JsonProperty("preparationInstructions", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("preparationInstructions")]
        public List<PreparationInstruction>? PreparationInstructions { get; set; }

        [JsonProperty("preparationType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("preparationType")]
        public string? PreparationType { get; set; }
    }

    public class FunctionalName
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class General
    {
        [JsonProperty("isFileForInternalUseOnly", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isFileForInternalUseOnly")]
        public string? IsFileForInternalUseOnly { get; set; }

        [JsonProperty("imageSource", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imageSource")]
        public string? ImageSource { get; set; }

        [JsonProperty("isTalentReleaseOnFile", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isTalentReleaseOnFile")]
        public string? IsTalentReleaseOnFile { get; set; }

        [JsonProperty("fileSize", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileSize")]
        public List<FileSize>? FileSize { get; set; }

        [JsonProperty("FileName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("FileName")]
        public string? FileName { get; set; }

        [JsonProperty("imageBackground", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imageBackground")]
        public string? ImageBackground { get; set; }

        [JsonProperty("fileFormatName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileFormatName")]
        public string? FileFormatName { get; set; }

        [JsonProperty("typeOfInformation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("typeOfInformation")]
        public string? TypeOfInformation { get; set; }

        [JsonProperty("fileEffectiveStartDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileEffectiveStartDate")]
        public DateTime? FileEffectiveStartDate { get; set; }

        [JsonProperty("fileVersion", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileVersion")]
        public string? FileVersion { get; set; }

        [JsonProperty("uniformResourceIdentifier", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("uniformResourceIdentifier")]
        public string? UniformResourceIdentifier { get; set; }

        [JsonProperty("fileSequenceNumber", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileSequenceNumber")]
        public string? FileSequenceNumber { get; set; }

        [JsonProperty("fileEffectiveEndDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileEffectiveEndDate")]
        public DateTime? FileEffectiveEndDate { get; set; }

        [JsonProperty("kroImageCaptureDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("kroImageCaptureDate")]
        public DateTime? KroImageCaptureDate { get; set; }
    }

    public class GlobalClassificationCategory
    {
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class GrossWeight
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Gs1TradeItemIdentificationKey
    {
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class GtinName
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Height
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ImageInfo
    {
        [JsonProperty("filePixelHeight", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("filePixelHeight")]
        public string? FilePixelHeight { get; set; }

        [JsonProperty("isFileBackgroundTransparent", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isFileBackgroundTransparent")]
        public string? IsFileBackgroundTransparent { get; set; }

        [JsonProperty("fileColourSchemeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("fileColourSchemeCode")]
        public string? FileColourSchemeCode { get; set; }

        [JsonProperty("filePixelWidth", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("filePixelWidth")]
        public string? FilePixelWidth { get; set; }
    }

    public class IndividualUnitMax
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class IndividualUnitMin
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class IngredientName
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class IngredientStatement
    {
        [JsonProperty("statement", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("statement")]
        public List<Statement>? Statement { get; set; }
    }

    public class IsDispatchUnit
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class IsInvoiceUnit
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class IsOrderableUnit
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Item
    {
        [JsonProperty("objId", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("objId")]
        public string? ObjId { get; set; }

        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("item", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("item")]
        public Item2? Item2 { get; set; }
    }

    public class Item2
    {
        [JsonProperty("baseUnitsPerPallet", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("baseUnitsPerPallet")]
        public List<BaseUnitsPerPallet>? BaseUnitsPerPallet { get; set; }

        [JsonProperty("hi", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("hi")]
        public string? Hi { get; set; }

        [JsonProperty("firstOrderDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("firstOrderDate")]
        public List<FirstOrderDate>? FirstOrderDate { get; set; }

        [JsonProperty("ingredientStatement", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ingredientStatement")]
        public List<IngredientStatement>? IngredientStatement { get; set; }

        [JsonProperty("minimumOrderQuantity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("minimumOrderQuantity")]
        public List<MinimumOrderQuantity>? MinimumOrderQuantity { get; set; }

        [JsonProperty("innerPack", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("innerPack")]
        public string? InnerPack { get; set; }

        [JsonProperty("distributionDetails", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("distributionDetails")]
        public List<DistributionDetail>? DistributionDetails { get; set; }

        [JsonProperty("gdsnRegistryStatus", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gdsnRegistryStatus")]
        public string? GdsnRegistryStatus { get; set; }

        [JsonProperty("dam", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("dam")]
        public List<Dam>? Dam { get; set; }

        [JsonProperty("measurementPrecisionOfNumberOfServingsPerPackage", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("measurementPrecisionOfNumberOfServingsPerPackage")]
        public string? MeasurementPrecisionOfNumberOfServingsPerPackage { get; set; }

        [JsonProperty("nonGTINPalletTi", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nonGTINPalletTi")]
        public List<NonGTINPalletTi>? NonGTINPalletTi { get; set; }

        [JsonProperty("globalClassificationCategory", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("globalClassificationCategory")]
        public GlobalClassificationCategory? GlobalClassificationCategory { get; set; }

        [JsonProperty("individualUnitMin", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("individualUnitMin")]
        public IndividualUnitMin? IndividualUnitMin { get; set; }

        [JsonProperty("brandOwnerName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("brandOwnerName")]
        public string? BrandOwnerName { get; set; }

        [JsonProperty("isBioengineeredDeclarationClaimRelevantDataProvided", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isBioengineeredDeclarationClaimRelevantDataProvided")]
        public string? IsBioengineeredDeclarationClaimRelevantDataProvided { get; set; }

        [JsonProperty("hasBatchNumber", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("hasBatchNumber")]
        public string? HasBatchNumber { get; set; }

        [JsonProperty("isVariableWeightItem", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isVariableWeightItem")]
        public string? IsVariableWeightItem { get; set; }

        [JsonProperty("netContent", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("netContent")]
        public List<NetContent>? NetContent { get; set; }

        [JsonProperty("foodAndBevIngredient", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("foodAndBevIngredient")]
        public List<FoodAndBevIngredient>? FoodAndBevIngredient { get; set; }

        [JsonProperty("doesTradeItemCarryUSDAChildNutritionLabel", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("doesTradeItemCarryUSDAChildNutritionLabel")]
        public string? DoesTradeItemCarryUSDAChildNutritionLabel { get; set; }

        [JsonProperty("productDescription", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("productDescription")]
        public List<ProductDescription>? ProductDescription { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("height")]
        public List<Height>? Height { get; set; }

        [JsonProperty("nutrientInformation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nutrientInformation")]
        public List<NutrientInformation>? NutrientInformation { get; set; }

        [JsonProperty("brandName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("brandName")]
        public string? BrandName { get; set; }

        [JsonProperty("functionalName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("functionalName")]
        public List<FunctionalName>? FunctionalName { get; set; }

        [JsonProperty("isBaseUnit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isBaseUnit")]
        public string? IsBaseUnit { get; set; }

        [JsonProperty("registryProgress", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("registryProgress")]
        public string? RegistryProgress { get; set; }

        [JsonProperty("nutrientFormatTypeCodeReference", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nutrientFormatTypeCodeReference")]
        public List<NutrientFormatTypeCodeReference>? NutrientFormatTypeCodeReference { get; set; }

        [JsonProperty("imEffectiveDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("imEffectiveDate")]
        public DateTime? ImEffectiveDate { get; set; }

        [JsonProperty("individualUnitMax", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("individualUnitMax")]
        public IndividualUnitMax? IndividualUnitMax { get; set; }

        [JsonProperty("externalFileLink", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("externalFileLink")]
        public List<ExternalFileLink>? ExternalFileLink { get; set; }

        [JsonProperty("isOrderableUnit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isOrderableUnit")]
        public List<IsOrderableUnit>? IsOrderableUnit { get; set; }

        [JsonProperty("shortDescription", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("shortDescription")]
        public List<ShortDescription>? ShortDescription { get; set; }

        [JsonProperty("informationProviderGLN", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("informationProviderGLN")]
        public string? InformationProviderGLN { get; set; }

        [JsonProperty("brandOwnerGLN", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("brandOwnerGLN")]
        public string? BrandOwnerGLN { get; set; }

        [JsonProperty("m2mHasSuppressedAttr", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("m2mHasSuppressedAttr")]
        public string? M2mHasSuppressedAttr { get; set; }

        [JsonProperty("dataCarrier", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("dataCarrier")]
        public List<DataCarrier>? DataCarrier { get; set; }

        [JsonProperty("volume", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("volume")]
        public List<Volume>? Volume { get; set; }

        [JsonProperty("firstShipDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("firstShipDate")]
        public List<FirstShipDate>? FirstShipDate { get; set; }

        [JsonProperty("netWeight", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("netWeight")]
        public List<NetWeight>? NetWeight { get; set; }

        [JsonProperty("minimumTradeItemLifespanFromProduction", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("minimumTradeItemLifespanFromProduction")]
        public List<MinimumTradeItemLifespanFromProduction>? MinimumTradeItemLifespanFromProduction { get; set; }

        [JsonProperty("packagingMarkedReturnable", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("packagingMarkedReturnable")]
        public string? PackagingMarkedReturnable { get; set; }

        [JsonProperty("minimumTradeItemLifespanFromArrival", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("minimumTradeItemLifespanFromArrival")]
        public List<MinimumTradeItemLifespanFromArrival>? MinimumTradeItemLifespanFromArrival { get; set; }

        [JsonProperty("gtinName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gtinName")]
        public List<GtinName>? GtinName { get; set; }

        [JsonProperty("productMarkedRecyclable", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("productMarkedRecyclable")]
        public string? ProductMarkedRecyclable { get; set; }

        [JsonProperty("foodAndBevDietTypeInfo", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("foodAndBevDietTypeInfo")]
        public List<FoodAndBevDietTypeInfo>? FoodAndBevDietTypeInfo { get; set; }

        [JsonProperty("totalQuantityOfNextLowerTradeItem", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("totalQuantityOfNextLowerTradeItem")]
        public string? TotalQuantityOfNextLowerTradeItem { get; set; }

        [JsonProperty("geneticallyModifiedDeclarationCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("geneticallyModifiedDeclarationCode")]
        public string? GeneticallyModifiedDeclarationCode { get; set; }

        [JsonProperty("alternateItemIdentification", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("alternateItemIdentification")]
        public List<AlternateItemIdentification>? AlternateItemIdentification { get; set; }

        [JsonProperty("marketingMessage", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("marketingMessage")]
        public List<MarketingMessage>? MarketingMessage { get; set; }

        [JsonProperty("registeredDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("registeredDate")]
        public DateTime? RegisteredDate { get; set; }

        [JsonProperty("startAvailabilityDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("startAvailabilityDate")]
        public List<StartAvailabilityDate>? StartAvailabilityDate { get; set; }

        [JsonProperty("countryOfOriginStatement", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("countryOfOriginStatement")]
        public List<CountryOfOriginStatement>? CountryOfOriginStatement { get; set; }

        [JsonProperty("informationProviderName", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("informationProviderName")]
        public string? InformationProviderName { get; set; }

        [JsonProperty("irradiatedCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("irradiatedCode")]
        public string? IrradiatedCode { get; set; }

        [JsonProperty("organicClaim", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("organicClaim")]
        public List<OrganicClaim>? OrganicClaim { get; set; }

        [JsonProperty("tradeItemContactInfo", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("tradeItemContactInfo")]
        public List<TradeItemContactInfo>? TradeItemContactInfo { get; set; }

        [JsonProperty("manufacturer", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("manufacturer")]
        public List<Manufacturer>? Manufacturer { get; set; }

        [JsonProperty("tradeItemTemperatureConditionTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("tradeItemTemperatureConditionTypeCode")]
        public string? TradeItemTemperatureConditionTypeCode { get; set; }

        [JsonProperty("targetMarket", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("targetMarket")]
        public string? TargetMarket { get; set; }

        [JsonProperty("isInvoiceUnit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isInvoiceUnit")]
        public List<IsInvoiceUnit>? IsInvoiceUnit { get; set; }

        [JsonProperty("additionalDescription", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("additionalDescription")]
        public List<AdditionalDescription>? AdditionalDescription { get; set; }

        [JsonProperty("itemStatus", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("itemStatus")]
        public string? ItemStatus { get; set; }

        [JsonProperty("numberOfServingsPerPackage", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("numberOfServingsPerPackage")]
        public string? NumberOfServingsPerPackage { get; set; }

        [JsonProperty("doesItemContainTargetMarketAttributes", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("doesItemContainTargetMarketAttributes")]
        public string? DoesItemContainTargetMarketAttributes { get; set; }

        [JsonProperty("foodAndBevPreparationInfo", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("foodAndBevPreparationInfo")]
        public List<FoodAndBevPreparationInfo>? FoodAndBevPreparationInfo { get; set; }

        [JsonProperty("numberOfItemsPerPallet", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("numberOfItemsPerPallet")]
        public List<NumberOfItemsPerPallet>? NumberOfItemsPerPallet { get; set; }

        [JsonProperty("isDispatchUnit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isDispatchUnit")]
        public List<IsDispatchUnit>? IsDispatchUnit { get; set; }

        [JsonProperty("productType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("productType")]
        public string? ProductType { get; set; }

        [JsonProperty("gdsnModifiedDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gdsnModifiedDate")]
        public DateTime? GdsnModifiedDate { get; set; }

        [JsonProperty("nonGTINPalletHi", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nonGTINPalletHi")]
        public List<NonGTINPalletHi>? NonGTINPalletHi { get; set; }

        [JsonProperty("gs1TradeItemIdentificationKey", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gs1TradeItemIdentificationKey")]
        public List<Gs1TradeItemIdentificationKey>? Gs1TradeItemIdentificationKey { get; set; }

        [JsonProperty("gtin", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gtin")]
        public string? Gtin { get; set; }

        [JsonProperty("typeOfItem", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("typeOfItem")]
        public string? TypeOfItem { get; set; }

        [JsonProperty("isConsumerUnit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("isConsumerUnit")]
        public string? IsConsumerUnit { get; set; }

        [JsonProperty("lastModifiedDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("lastModifiedDate")]
        public DateTime? LastModifiedDate { get; set; }

        [JsonProperty("consumerStorageInstructions", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("consumerStorageInstructions")]
        public List<ConsumerStorageInstruction>? ConsumerStorageInstructions { get; set; }

        [JsonProperty("productInformationDetail", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("productInformationDetail")]
        public List<ProductInformationDetail>? ProductInformationDetail { get; set; }

        [JsonProperty("packagingDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("packagingDate")]
        public List<PackagingDate>? PackagingDate { get; set; }

        [JsonProperty("growingMethodCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("growingMethodCode")]
        public List<string>? GrowingMethodCode { get; set; }

        [JsonProperty("grossWeight", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("grossWeight")]
        public List<GrossWeight>? GrossWeight { get; set; }

        [JsonProperty("depth", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("depth")]
        public List<Depth>? Depth { get; set; }

        [JsonProperty("tradeItemTemperatureInformation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("tradeItemTemperatureInformation")]
        public List<TradeItemTemperatureInformation>? TradeItemTemperatureInformation { get; set; }

        [JsonProperty("ti", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ti")]
        public string? Ti { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("width")]
        public List<Width>? Width { get; set; }

        [JsonProperty("allergenRelatedInformation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("allergenRelatedInformation")]
        public List<AllergenRelatedInformation>? AllergenRelatedInformation { get; set; }

        [JsonProperty("countryOfOrigin", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("countryOfOrigin")]
        public List<CountryOfOrigin>? CountryOfOrigin { get; set; }

        [JsonProperty("packagingInformation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("packagingInformation")]
        public List<PackagingInformation>? PackagingInformation { get; set; }

        [JsonProperty("quantityOfNextLevelWithinInnerPack", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("quantityOfNextLevelWithinInnerPack")]
        public string? QuantityOfNextLevelWithinInnerPack { get; set; }

        [JsonProperty("effectiveDate", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [JsonProperty("nonPackagedSizeDimension", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nonPackagedSizeDimension")]
        public List<NonPackagedSizeDimension>? NonPackagedSizeDimension { get; set; }

        [JsonProperty("tradeItemFeatureBenefitExtended", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("tradeItemFeatureBenefitExtended")]
        public List<TradeItemFeatureBenefitExtended>? TradeItemFeatureBenefitExtended { get; set; }

        [JsonProperty("tradeItemKeyWords", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("tradeItemKeyWords")]
        public List<TradeItemKeyWord>? TradeItemKeyWords { get; set; }
    }

    public class Manufacturer
    {
        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class MarketingMessage
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("tradeItemMarketingMessage", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("tradeItemMarketingMessage")]
        public List<TradeItemMarketingMessage>? TradeItemMarketingMessage { get; set; }
    }

    public class MaximumTemperature
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class MinimumOrderQuantity
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class MinimumTemperature
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class MinimumTradeItemLifespanFromArrival
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class MinimumTradeItemLifespanFromProduction
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NetContent
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NetWeight
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NonGTINPalletHi
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NonGTINPalletTi
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NonPackagedSizeDimension
    {
        [JsonProperty("descriptiveSizeDimension", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("descriptiveSizeDimension")]
        public List<DescriptiveSizeDimension>? DescriptiveSizeDimension { get; set; }

        [JsonProperty("sizeSystemCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("sizeSystemCode")]
        public List<string>? SizeSystemCode { get; set; }

        [JsonProperty("sizeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("sizeCode")]
        public List<string>? SizeCode { get; set; }
    }

    public class NumberOfItemsPerPallet
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NumberOfUnitInShippingContainer
    {
        [JsonProperty("shippingContainerQuantityDescription", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("shippingContainerQuantityDescription")]
        public List<ShippingContainerQuantityDescription>? ShippingContainerQuantityDescription { get; set; }
    }

    public class NutrientBasisQuantity
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class NutrientDetail
    {
        [JsonProperty("quantityContained", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("quantityContained")]
        public List<QuantityContained>? QuantityContained { get; set; }

        [JsonProperty("nutrientTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nutrientTypeCode")]
        public string? NutrientTypeCode { get; set; }

        [JsonProperty("nutrientValueDerivationCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nutrientValueDerivationCode")]
        public string? NutrientValueDerivationCode { get; set; }

        [JsonProperty("measurementPrecisionCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("measurementPrecisionCode")]
        public string? MeasurementPrecisionCode { get; set; }

        [JsonProperty("dailyValueIntakePercent", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("dailyValueIntakePercent")]
        public string? DailyValueIntakePercent { get; set; }

        [JsonProperty("dailyValueIntakePercentMeasurementPrecisionCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("dailyValueIntakePercentMeasurementPrecisionCode")]
        public string? DailyValueIntakePercentMeasurementPrecisionCode { get; set; }
    }

    public class NutrientFormatTypeCodeReference
    {
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    public class NutrientInformation
    {
        [JsonProperty("nutrientBasisQuantityTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nutrientBasisQuantityTypeCode")]
        public string? NutrientBasisQuantityTypeCode { get; set; }

        [JsonProperty("servingSizeDescription", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("servingSizeDescription")]
        public List<ServingSizeDescription>? ServingSizeDescription { get; set; }

        [JsonProperty("nutrientBasisQuantity", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nutrientBasisQuantity")]
        public NutrientBasisQuantity? NutrientBasisQuantity { get; set; }

        [JsonProperty("preparationStateCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("preparationStateCode")]
        public string? PreparationStateCode { get; set; }

        [JsonProperty("servingSize", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("servingSize")]
        public List<ServingSize>? ServingSize { get; set; }

        [JsonProperty("nutrientDetail", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("nutrientDetail")]
        public List<NutrientDetail>? NutrientDetail { get; set; }
    }

    public class OrderingLeadTime
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class OrganicClaim
    {
        [JsonProperty("organicTradeItemCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("organicTradeItemCode")]
        public string? OrganicTradeItemCode { get; set; }
    }

    public class PackagingDate
    {
        [JsonProperty("tradeItemDateOnPackagingTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("tradeItemDateOnPackagingTypeCode")]
        public string? TradeItemDateOnPackagingTypeCode { get; set; }
    }

    public class PackagingInformation
    {
        [JsonProperty("packagingTypeCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("packagingTypeCode")]
        public string? PackagingTypeCode { get; set; }

        [JsonProperty("numberOfUnitInShippingContainer", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("numberOfUnitInShippingContainer")]
        public List<NumberOfUnitInShippingContainer>? NumberOfUnitInShippingContainer { get; set; }
    }

    public class PreparationInstruction
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ProductDescription
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ProductInformationDetail
    {
        [JsonProperty("claimDetail", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("claimDetail")]
        public List<ClaimDetail>? ClaimDetail { get; set; }
    }

    public class QuantityContained
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ServingSize
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ServingSizeDescription
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ServingSuggestion
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ShippingContainerQuantityDescription
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ShortDescription
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class StartAvailabilityDate
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public DateTime? Value { get; set; }
    }

    public class Statement
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class TargetMarketCommunicationChannel
    {
        [JsonProperty("communicationChannel", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("communicationChannel")]
        public List<CommunicationChannel>? CommunicationChannel { get; set; }
    }

    public class TradeItemContactInfo
    {
        [JsonProperty("targetMarketCommunicationChannel", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("targetMarketCommunicationChannel")]
        public List<TargetMarketCommunicationChannel>? TargetMarketCommunicationChannel { get; set; }

        [JsonProperty("contactType", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("contactType")]
        public string? ContactType { get; set; }
    }

    public class TradeItemFeatureBenefitExtended
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class TradeItemKeyWord
    {
        [JsonProperty("gln", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("gln")]
        public string? Gln { get; set; }

        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class TradeItemMarketingMessage
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class TradeItemTemperatureInformation
    {
        [JsonProperty("maximumTemperature", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("maximumTemperature")]
        public MaximumTemperature? MaximumTemperature { get; set; }

        [JsonProperty("temperatureQualifierCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("temperatureQualifierCode")]
        public string? TemperatureQualifierCode { get; set; }

        [JsonProperty("minimumTemperature", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("minimumTemperature")]
        public MinimumTemperature? MinimumTemperature { get; set; }
    }

    public class Volume
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class Width
    {
        [JsonProperty("qual", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("qual")]
        public string? Qual { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }


}