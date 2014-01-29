﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace Crm
{
    [TestFixture]
    public class ClientTransferTest
    {
        private Client client;
        private Manager firstManager;
        private Manager targetManager;
        private Department salesDepartment;
        private Department supportDepartment;

        [SetUp]
        public void SetUp()
        {
            salesDepartment = new Department("Отдел продаж");
            supportDepartment = new Department("Поддержка");
            
            firstManager = new Manager("Петров", salesDepartment, Position.Manager);
            client = new Client { Name = "ООО Вектор Плюс" };
            firstManager.AddClient(client);
            
            targetManager = new Manager("Сидоров", salesDepartment, Position.Manager);
        }

        [Test]
        public void should_transfer_client_from_one_manager_to_another()
        {
            firstManager.TransferClientTo(client, targetManager);
            
            firstManager.GetClients().Should().BeEmpty();
            targetManager.GetClients().ShouldAllBeEquivalentTo(new List<Client> {client});
        }

        [Test]
        public void cant_transfer_client_which_doesnt_belong_to_manager()
        {
            firstManager.RemoveClient(client);
            var anotherManager = new Manager("Новиков", salesDepartment, Position.Manager);
            anotherManager.AddClient(client);
            
            firstManager.Invoking(m => m.TransferClientTo(client, targetManager))
                .ShouldThrow<TransferClientDeniedException>();
        }

        [Test]
        public void chief_of_manager_can_transfer_his_client()
        {
            var chief = new Manager("Иванов", salesDepartment, Position.DepartmentChief);

            chief.TransferClientTo(client, targetManager);

            firstManager.GetClients().Should().BeEmpty();
            targetManager.GetClients().ShouldAllBeEquivalentTo(new List<Client> { client });
        }

        [Test]
        public void chief_of_another_department_cant_transfer_client()
        {
            var chief = new Manager("Иванов", supportDepartment, Position.DepartmentChief);
            
            chief.Invoking(m => m.TransferClientTo(client, targetManager))
                .ShouldThrow<TransferClientDeniedException>();
        }
    }
}