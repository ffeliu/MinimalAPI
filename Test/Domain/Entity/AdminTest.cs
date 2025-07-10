using minimalapi.domain.entity;

namespace Test.Domain.Entity;

[TestClass]
public class AdminTest
{
    [TestMethod]
    public void TestarGetSetProperties()
    {
        // Arrange
        var admin = new Admin();

        // Act
        admin.Id = 1;
        admin.Name = "Admin Name";
        admin.Email = "admin@example.com";
        admin.Profile = "Editor";

        // Assert
        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual("Admin Name", admin.Name);
        Assert.AreEqual("admin@example.com", admin.Email);
        Assert.AreEqual("Editor", admin.Profile);
    }
}