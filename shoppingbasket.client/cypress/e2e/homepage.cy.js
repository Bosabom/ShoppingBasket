context('Basket form and GeneratedReceipt integration', () => {
  beforeEach(() => {
    // Visit home page where BasketForm is mounted
    cy.visit('/');
  });

  it('visited my home page', () => {
    cy.visit('/')
  })

  it('shows validation errors for required fields', () => {
    // cause validation: type negative values and submit
    cy.get('#soup-quantity').clear().type(' ');
    cy.get('#bread-quantity').clear().type(' ');
    cy.get('#milk-quantity').clear().type(' ');
    cy.get('#apples-quantity').clear().type(' ');

    cy.get('#button-submit-form').click();

    // Validation messages are shown for min constraint
    cy.contains('Soup quantity is required!').should('exist');
    cy.contains('Bread quantity is required!').should('exist');
    cy.contains('Milk quantity is required!').should('exist');
    cy.contains('Apples quantity is required!').should('exist');
  });

  it('shows validation errors for negative quantities and required fields', () => {
    // cause validation: type negative values and submit
    cy.get('#soup-quantity').clear().type('-1');
    cy.get('#bread-quantity').clear().type('-2');
    cy.get('#milk-quantity').clear().type('-3');
    cy.get('#apples-quantity').clear().type('-4');

    cy.get('#button-submit-form').click();

    // Validation messages are shown for min constraint
    cy.contains('Soup quantity should be greater than 0!').should('exist');
    cy.contains('Bread quantity should be greater than 0!').should('exist');
    cy.contains('Milk quantity should be greater than 0!').should('exist');
    cy.contains('Apples quantity should be greater than 0!').should('exist');
  });

  it('submits form and displays stubbed receipt (mocking response)', () => {
    // Arrange: stub POST /receipts response
    const createdReceipt = {
      receiptId: 1,
      receiptNumber: '00000001',
      createdDateTime: new Date().toISOString(),
      totalCost: 17.20,
      itemsOrdered: [
        {
          itemOrderedId: 1,
          itemId: 1,
          itemDescription: "Soup",
          quantity: 2,
          subTotalCost: 1.3,
          discountedCost: null,
          totalCost: 1.3
        },
        {
          itemOrderedId: 2,
          itemId: 2,
          itemDescription: "Bread",
          quantity: 1,
          subTotalCost: 0.8,
          discountedCost: 0.4,
          totalCost: 0.4
        },
        {
          itemOrderedId: 3,
          itemId: 3,
          itemDescription: "Milk",
          quantity: 5,
          subTotalCost: 6.5,
          discountedCost: null,
          totalCost: 6.5
        },
        {
          itemOrderedId: 4,
          itemId: 4,
          itemDescription: "Apples",
          quantity: 10,
          subTotalCost: 10,
          discountedCost: null,
          totalCost: 9
        }
      ]
    };

    cy.intercept('POST', '/receipts', {
      statusCode: 201,
      body: createdReceipt
    }).as('createReceipt');

    // Act: fill form and submit (2 soups + 1 bread + 5l milk + 10 apple bags)
    cy.get('#soup-quantity').clear().type('2');
    cy.get('#bread-quantity').clear().type('1');
    cy.get('#milk-quantity').clear().type('5');
    cy.get('#apples-quantity').clear().type('10');

    cy.get('#button-submit-form').click();

    // wait for request and ensure UI shows GeneratedReceipt
    cy.wait('@createReceipt');

    // Generated receipt wrapper should be visible
    cy.get('.generated-receipt-wrapper').should('exist');

    // Assertions on receipt header info
    cy.contains('Number:').parent().should('contain.text', createdReceipt.receiptNumber);
    cy.contains('Total:').parent().should('contain.text', '€17.20');

    // Check receipt table rows exist and contain expected data
    cy.get('.receipt-table tbody tr').should('have.length', createdReceipt.itemsOrdered.length);
    cy.get('.receipt-table tbody tr').first().within(() => {
      cy.contains('Soup').should('exist');
      cy.contains('2').should('exist');
      cy.contains('€1.30').should('exist');
    });

    // discounted cell for bread should show discounted value
    cy.get('.receipt-table tbody tr').eq(1).within(() => {
      cy.contains('Bread').should('exist');
      cy.contains('1').should('exist');
      cy.contains('€0.80').should('exist'); // subtotalCost shown
      cy.contains('€0.40').should('exist'); // discountedCost shown
      cy.contains('€0.40').should('exist'); // line total
    });

    // discounted cell for apple should show discounted value
    cy.get('.receipt-table tbody tr').eq(3).within(() => {
      cy.contains('Apples').should('exist');
      cy.contains('10').should('exist');
      cy.contains('€10').should('exist'); // subtotalCost shown
      cy.contains('€9').should('exist'); // discountedCost shown
      cy.contains('€9').should('exist'); // line total
    });
  });
});