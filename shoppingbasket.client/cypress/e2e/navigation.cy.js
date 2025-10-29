context('Top navigation links', () => {
  it('navigates to history via History link', () => {
    cy.visit('/');

    cy.get('#history-link').click();

    // Ensure URL contains /history and page loads (ReceiptsTable)
    cy.url().should('include', '/history');
    cy.get('#tableLabel').should('exist');
  });

  it('navigates back to home via Home link', () => {
    cy.visit('/history');

    cy.get('#home-link').click();
    cy.url().should('match', /\/(index.html)?$/);
    // Basket form submit button visible on home
    cy.get('#button-submit-form').should('exist');
  });
});