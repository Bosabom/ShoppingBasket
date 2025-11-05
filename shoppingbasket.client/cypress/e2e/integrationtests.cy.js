context('Integration tests with API requests', () => {

    //POST new receipt with validation error
    it('displays server error message when API returns 400 with problem details', () => {
        cy.visit('/');
        cy.fixture('formSubmitWithZeros.json').then((payloadWithZeros) => {
            cy.request({
                method: 'POST',
                url: '/receipts',
                body: payloadWithZeros,
                headers: {
                    'Content-Type': 'application/json'
                },
                failOnStatusCode: false
            })
                //Assert
                .then((response) => {
                    expect(response.status).to.eq(400);
                    expect(response.body.detail).to.eq('At least one item quantity must be greater than zero.');
                });
        });
    });

    //POST new receipt with success
    it('created succesfully new receipt with totalCost=15.30', () => {
        cy.visit('/');
        cy.fixture('formSubmit.json').then((payload) => {
            cy.request({
                method: 'POST',
                url: '/receipts',
                body: payload,
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then((response) => {
                    //Assert
                    expect(response.status).to.eq(201);
                    expect(response.body).to.have.property('receiptId');
                    expect(response.body).to.have.property('receiptNumber');
                    expect(response.body).to.have.property('totalCost');
                    expect(response.body.totalCost).to.eq(15.30);
                    expect(response.body).to.have.property('itemsOrdered');
                    expect(response.body.itemsOrdered).to.have.length(4);
                });
        });
    });

    //GET history of receipts
    it('get past transactions and display them in history table', () => {
        cy.visit('/history');
        cy.request('/receipts/history').as('receipts')

        cy.get('@receipts').should((response => {
            //Assert
            expect(response.status).to.eq(200);
            expect(response.body).to.be.an('array');
            expect(response.body).to.have.lengthOf.at.least(1);
            expect(response.body[0]).to.have.property('receiptId');
            expect(response.body[0]).to.have.property('receiptNumber');
            expect(response.body[0]).to.have.property('totalCost');
            expect(response.body[0]).to.have.property('createdDateTime');
        }))
    });
});