using System;
using ForVlad.Data;
using ForVlad.Models;

class TestCompilation
{
    static void Main()
    {
        Console.WriteLine("Testing SqlDataService interface implementation...");
        
        // Test that SqlDataService implements ISimpleDataService
        ISimpleDataService service = new SqlDataService();
        
        // Test all methods exist
        Console.WriteLine("✓ SqlDataService implements ISimpleDataService");
        Console.WriteLine("✓ All interface methods are implemented");
        
        Console.WriteLine("\nInterface methods:");
        Console.WriteLine("- GetCounterparties");
        Console.WriteLine("- GetCounterparty");
        Console.WriteLine("- SaveCounterparty");
        Console.WriteLine("- DeleteCounterparty");
        Console.WriteLine("- GetAssets");
        Console.WriteLine("- GetAsset");
        Console.WriteLine("- SaveAsset");
        Console.WriteLine("- DeleteAsset");
        Console.WriteLine("- GetContracts");
        Console.WriteLine("- GetContract");
        Console.WriteLine("- SaveContract");
        Console.WriteLine("- DeleteContract");
        Console.WriteLine("- GetPaymentSchedules");
        Console.WriteLine("- GetSpecifications");
        Console.WriteLine("- MarkPaymentPaid");
        Console.WriteLine("- GetPaymentReport");
        Console.WriteLine("- GetAssetUtilizationReport");
        Console.WriteLine("- InitializeTestData");
        Console.WriteLine("- ResetDemoData");
        Console.WriteLine("- TestConnection");
        Console.WriteLine("- GenerateContractNumber");
        
        Console.WriteLine("\n✓ Compilation test passed!");
    }
}
