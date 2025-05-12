namespace MODBD_Api.Common.Contracts;

public static class ApiRoutes
{
    private const string _root = "api";

    public static class Insights 
    {
        private const string _insightsRoute = _root + "/insights";

        public const string Tag = "Insights";

        public const string GetDynamic = _insightsRoute + "/dynamic";
    }

    
    public static class Identity
    {
        private const string _identityRoute = _root + "/identity";

        public const string Tag = "Identity";

        public const string GetRegions = _identityRoute + "/regions";
        public const string GetRegion = _identityRoute + "/regions/{id}";

        public const string Register = _identityRoute + "/register";
        public const string ConfirmRegistration = _identityRoute + "/confirm-registration";
        public const string JwtLoginWithUsername = _identityRoute + "/jwt/login-with-username";
        public const string JwtLoginWithEmail = _identityRoute + "/jwt/login-with-email";
        public const string RefreshAccessJwt = _identityRoute + "/jwt/refresh";
    }

    public static class Purchases
    {
        private const string _purchasesRoute = _root + "/purchases";

        public const string Tag = "Purchases";

        public const string GetProducts = _purchasesRoute + "/products";
        public const string GetProduct = _purchasesRoute + "/products/{id}";
        public const string GetVendors = _purchasesRoute + "/vendors";
        public const string GetVendor = _purchasesRoute + "/vendors/{id}";
    }

    public static class Sales
    {
        private const string _salesRoute = _root + "/sales";

        public const string Tag = "Sales";

        public const string GetOrders = _salesRoute + "/{tenantId}/orders";
        public const string GetOrder = _salesRoute + "/{tenantId}/orders/{id}";
        public const string GetMyOrders = _salesRoute + "/{tenantId}/{customerId}/my-orders";
        public const string PlaceOrder = _salesRoute + "/{tenantId}/orders";
        public const string GetProducts = _salesRoute + "/{tenantId}/products";
    }

    public static class Billing
    {
        private const string _billingRoute = _root + "/billing";

        public const string Tag = "Billing";

        public const string GetInvoices = _billingRoute + "/invoices";
        public const string GetInvoice = _billingRoute + "/invoices/{id}";
        public const string GetMyInvoices = _billingRoute + "{customerId}/my-invoices";
    }
}
