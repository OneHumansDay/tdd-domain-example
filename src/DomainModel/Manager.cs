﻿namespace Crm
{
    using System.Collections.Generic;

    public class Manager
    {
        private readonly List<Client> clients;

        public Manager(string name, Department department, Position position)
        {
            clients = new List<Client>();
            Name = name;
            Department = department;
            Position = position;
            ManagerBonus = new ManagerBonus(this);
        }

        public string Name { get; private set; }

        public Department Department { get; private set; }

        public Position Position { get; private set; }

        internal ManagerBonus ManagerBonus { get; set; }

        public void AddClient(Client client)
        {
            client.SetManager(this);
            clients.Add(client);
        }

        public IEnumerable<Client> GetClients()
        {
            return clients.AsReadOnly();
        }

        public void TransferClientTo(Client client, Manager targetManager)
        {
            if (IsManagerFromSameDepartment(targetManager) == false)
                throw new TransferOutsiteTheDepartmentException(this, targetManager, client);

            if (IsChiefOfClientManagerDepartment(client))
            {
                client.TransferTo(targetManager);
                return;
            }

            if (BelongsToThisManager(client) == false)
                throw new TransferClientDeniedException(this, client);

            RemoveClient(client);

            targetManager.AddClient(client);

            ManagerBonus.IncreaseForClientTransfer();
        }

        private bool IsManagerFromSameDepartment(Manager manager)
        {
            return Department == manager.Department;
        }

        private void RemoveClient(Client client)
        {
            clients.Remove(client);
            client.ClearManager();
        }

        private bool IsChiefOfClientManagerDepartment(Client client)
        {
            if (client.Manager == null)
                return false;

            return Position == Position.DepartmentChief && Department == client.Manager.Department;
        }

        private bool BelongsToThisManager(Client client)
        {
            return client.Manager == this;
        }
    }
}