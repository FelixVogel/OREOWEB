// Main site code

capsule(() => {
    queryForEach('div[id^="rating_"]', (ratingContainer) => {
        const containerQuery = `div[id="${ratingContainer.id}"] container.oreo-rating-stars-dynamic`;

        query(containerQuery).addEventListener('mouseleave', (e) => {
            queryForEach(`${containerQuery} span`, (ratingScore) => {
                ratingScore['hovered'] = false;
                ratingScore.setAttribute('active', 'false');
            });
        });

        queryForEach(`${containerQuery} span`, (ratingScore) => {
            ratingScore.addEventListener('click', (e) => {
                let oreoId = ratingContainer.id.substr(7);

                ajax({
                    url: `/api/Rating?oreoId=${oreoId}&score=${(<HTMLElement>e.target).innerText}`,
                    method: 'POST',
                    response: (res) => {
                        let response = res.asJSON();

                        if (!response) return;

                        let id: number = response['oreoId'];
                        let score = parseFloat((response['score'] / response['total']).toFixed(2));

                        query(`#rating_score_${id}`).innerText = `(${score})`;

                        queryForEach(`div[id="rating_${id}"] container.oreo-rating-stars-static span`, (element, i) => {
                            if (i + 1 <= score) element.setAttribute('active', 'true');
                            else element.setAttribute('active', 'false');
                        });
                    }
                });
            });

            ratingScore.addEventListener('mouseenter', (e) => {
                ratingScore['hovered'] = true;
                ratingScore.setAttribute('active', 'true');

                let d = false;

                queryForEach(`${containerQuery} span`, (ratingScore) => {
                    if (!ratingScore['hovered'] && !d) {
                        ratingScore.setAttribute('active', 'true');
                    } else {
                        d = true;
                    }
                });
            });

            ratingScore.addEventListener('mouseleave', (e) => {
                ratingScore['hovered'] = false;
                ratingScore.setAttribute('active', 'false');
            });
        });
    });

    query('#alogin')?.addEventListener('click', (e) => query('#asubmit').click());
});
