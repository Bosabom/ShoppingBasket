context('Receipts history page', () => {

  it('visited history page', () => {
    cy.visit('/history')
  })

  it('renders receipts table from /receipts/history (mocking response)', () => {
    const receipts = [
      {
        receiptId: 101,
        receiptNumber: '00000101',
        createdDateTime: '2025-10-29T12:00:00Z',
        totalCost: 2.10
      },
      {
        receiptId: 102,
        receiptNumber: '00000102',
        createdDateTime: '2025-10-28T09:30:00Z',
        totalCost: 3.50
      }
    ];

    // intercept history GET and return the list
    cy.intercept('GET', '/receipts/history', { statusCode: 200, body: receipts }).as('getHistory');

    // Visit history route
    cy.visit('/history');

    // Wait for the call and assert table rendering
    cy.wait('@getHistory');

    // Table header exists
    cy.get('#tableLabel').should('exist').and('contain.text', 'History of receipts');

    // Two rows rendered
    cy.get('.history-table tbody tr').should('have.length', receipts.length);

    // Check first row values
    cy.get('.history-table tbody tr').first().within(() => {
      cy.contains(receipts[0].receiptNumber).should('exist');
      cy.contains('€2.10').should('exist');
      cy.contains('2025').should('exist'); // created date exercise (year)
    });
  });

  it('shows loading placeholder when receipts undefined and then renders (mocking response)', () => {
    // Use a delayed response to observe loading message
    cy.intercept('GET', '/receipts/history', (req) => {
      req.on('response', (res) => { });
      // delay the response slightly
      req.reply((res) => {
        res.send({ statusCode: 200, body: [], delay: 200 });
      });
    }).as('delayedHistory');

    cy.visit('/history');

    // loading placeholder present
    cy.contains('Loading... Please wait').should('exist');

    cy.wait('@delayedHistory');

    // after response an empty table body should render (no rows)
    cy.get('.history-table tbody tr').should('have.length', 0);
  });
});